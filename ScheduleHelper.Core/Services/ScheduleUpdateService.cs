using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleHelper.Core.Domain.Entities.Helpers;
using ScheduleHelper.Core.ServiceContracts;

namespace ScheduleHelper.Core.Services
{
    public class ScheduleUpdateService:IScheduleUpdateService
    {            
        
        
        IScheduleRepository _scheduleRepository;

        public ScheduleUpdateService(IScheduleRepository scheduleRepository)
        {


            _scheduleRepository = scheduleRepository;

        }

        public async Task FinaliseTimeSlot(FinaliseSlotDTO model)
        {



            TimeSlotInSchedule? timeSlot = await _scheduleRepository.GetTimeSlot(model.SlotId);
            bool isTimeSlotIdExistInDb = timeSlot != null;
            if (!isTimeSlotIdExistInDb)
            {
                throw new ArgumentException("time slot with such id isn't present on list");
            }


            await changeSlotStatus(timeSlot, TimeSlotStatus.Finished);

            await updateTimeSlotsOnListBasedOnFinishTime(model, timeSlot);

        }

        private async Task updateTimeSlotsOnListBasedOnFinishTime(FinaliseSlotDTO model, TimeSlotInSchedule? timeSlot)
        {
            TimeOnly actualFinishTime = TimeOnly.Parse(model.FinishTime);
            bool timeSlotFirstOnList =await checkIsTimeSlotFirstOnList(timeSlot);
            var scheduleSettings = await _scheduleRepository.GetScheduleSettings();
            if (!timeSlotFirstOnList)
            {
                await updateSlotsWhenfinishedSlotNotFirstOnList(timeSlot, actualFinishTime, scheduleSettings);
                return;
            }


            bool isTimeSlotFinishedOnTime = timeSlot.FinishTime.AreTimesEqualWithTolerance(actualFinishTime);
            if (isTimeSlotFinishedOnTime)
            {
                return;
            }

            

            

            await updateSlotsTimesWithDelay(timeSlot, actualFinishTime, scheduleSettings);
          

            

        }

        private async Task updateSlotsWhenfinishedSlotNotFirstOnList(TimeSlotInSchedule? finishedSlot, TimeOnly actualFinishTime, ScheduleSettings scheduleSettings)
        {
            List<TimeSlotInSchedule> activeSlots = await _scheduleRepository.GetActiveSlots();
            var slotsBeforeFinishedSlot = activeSlots.Where(slot => slot.StartTime < finishedSlot.StartTime).ToList();
            var slotsAfterFinishedSlot = activeSlots.Where(slot => slot.StartTime >= finishedSlot.StartTime).ToList();
            TimeSlotInSchedule? breakBeforeFinishedSlot = null;

            if (!finishedSlot.IsItBreak)
            {
                breakBeforeFinishedSlot = getSlotBeforeTimeSlot(slotsBeforeFinishedSlot, finishedSlot); ;
            }

         

            TimeSlotInSchedule earliestSlot = getEarliestTimeSlot(slotsBeforeFinishedSlot);
            double dely = getDelyFromStartTime(earliestSlot, actualFinishTime, scheduleSettings.StartTime);
            double delyForSlotsBeforeFinishedSlot = dely + breakBeforeFinishedSlot.getDurationOfSlotInMin();

 
            double delyForforSlotsAfterFinshedSlot = dely - finishedSlot.getDurationOfSlotInMin();


            foreach (var slot in slotsBeforeFinishedSlot)
            {
                bool enoughTimeForSlot = true;
                if (slot.Id==breakBeforeFinishedSlot.Id)
                {
                    
                    double brekaDuration = slot.getDurationOfSlotInMin();
                    var breakStartTime = actualFinishTime;
                    if (actualFinishTime < scheduleSettings.StartTime)
                        breakStartTime = scheduleSettings.StartTime;
                    slot.StartTime = breakStartTime;
                    slot.FinishTime = breakStartTime.AddMinutes(brekaDuration);
                    enoughTimeForSlot = checkIfIsEnoughTimeForTimeSlot(scheduleSettings, 0, slot);
                    
                    if (!enoughTimeForSlot)
                    {
                        slot.Status = TimeSlotStatus.Canceled;
                    }


                    await _scheduleRepository.UpdateTimeSlot(slot);
                    continue;
                }
                enoughTimeForSlot = checkIfIsEnoughTimeForTimeSlot(scheduleSettings, delyForSlotsBeforeFinishedSlot, slot);
                if (!enoughTimeForSlot)
                {
                    slot.Status = TimeSlotStatus.Canceled;
                    await _scheduleRepository.UpdateTimeSlot(slot);
                    continue;
                }
                slot.updateStartAndFinishWithDely(delyForSlotsBeforeFinishedSlot);
                await _scheduleRepository.UpdateTimeSlot(slot);

            }

            foreach (var slot in slotsAfterFinishedSlot)
            {
                bool enoughTimeForSlot = checkIfIsEnoughTimeForTimeSlot(scheduleSettings, delyForforSlotsAfterFinshedSlot, slot);
                if (!enoughTimeForSlot)
                {
                    slot.Status = TimeSlotStatus.Canceled;
                    await _scheduleRepository.UpdateTimeSlot(slot);
                    continue;
                }
                slot.updateStartAndFinishWithDely(delyForforSlotsAfterFinshedSlot);
                await _scheduleRepository.UpdateTimeSlot(slot);
            }


        }

        private double getDelyFromStartTime(TimeSlotInSchedule timeSlot, TimeOnly actualFinishTime, TimeOnly scheduleStartTime)
        {
            if (timeSlot.StartTime < actualFinishTime)
                return (actualFinishTime - timeSlot.StartTime).TotalMinutes;

            if (scheduleStartTime < actualFinishTime)
                return -(timeSlot.StartTime - actualFinishTime).TotalMinutes;

            return -(timeSlot.StartTime - scheduleStartTime).TotalMinutes;
        }

        private TimeSlotInSchedule getEarliestTimeSlot(List<TimeSlotInSchedule> slots)
        {
            TimeSlotInSchedule earliestSlot = slots[0];
            foreach(var slot in slots) {
                if(earliestSlot.StartTime>slot.StartTime)
                {
                    earliestSlot = slot;
                }
            }
            return earliestSlot;
        }

        

        private TimeSlotInSchedule getSlotBeforeTimeSlot(List<TimeSlotInSchedule> slotBeforeFinishedTimeSlot, TimeSlotInSchedule timeSlot)
        {
            var closestSlot = slotBeforeFinishedTimeSlot[0];
            foreach (var slot in slotBeforeFinishedTimeSlot)
            {
                if(closestSlot.StartTime<slot.StartTime)
                {
                    closestSlot = slot;
                }
            }
            return closestSlot;
        }

        private async Task<bool> checkIsTimeSlotFirstOnList(TimeSlotInSchedule? timeSlot)
        {
            List<TimeSlotInSchedule> activeSlots = await _scheduleRepository.GetActiveSlots();

            return !activeSlots.Any(slot => slot.StartTime < timeSlot.StartTime);
        }

        private async Task updateSlotsTimesWithDelay(TimeSlotInSchedule? finishedTimeSlot, TimeOnly actualFinishTime, ScheduleSettings scheduleSettings)
        {
            double dely = getDely(finishedTimeSlot, actualFinishTime,scheduleSettings.StartTime);
            List<TimeSlotInSchedule> activeSlots = await _scheduleRepository.GetActiveSlots();


            foreach (var slot in activeSlots)
            {
                if (slot.Id == finishedTimeSlot.Id || finishedTimeSlot.StartTime > slot.StartTime)
                {
                    continue;
                }

                
                bool enoughTimeToFinishSlot = checkIfIsEnoughTimeForTimeSlot(scheduleSettings, dely, slot);
                if (!enoughTimeToFinishSlot)
                {
                    await changeSlotStatus(slot, TimeSlotStatus.Canceled);
                    continue;
                }

                slot.StartTime = slot.StartTime.AddMinutes(dely);
                slot.FinishTime = slot.FinishTime.AddMinutes(dely);
                await _scheduleRepository.UpdateTimeSlot(slot);

            }
        }

        private static bool checkIfIsEnoughTimeForTimeSlot(ScheduleSettings scheduleSettings, double dely, TimeSlotInSchedule slot)
        {
            TimeSpan timeSpan = new TimeOnly(23, 59) - slot.FinishTime;
            bool slotShouldBeFinishedInNextDay = timeSpan.TotalMinutes < dely;
            bool notEnoughTimeToFinishSlot = slot.FinishTime.AddMinutes(dely) > scheduleSettings.FinishTime || slotShouldBeFinishedInNextDay;
            return !notEnoughTimeToFinishSlot;
        }

        private static double getDely(TimeSlotInSchedule? timeSlot, TimeOnly actualFinishTime, TimeOnly scheduleStartTime)
        {
            if (timeSlot.FinishTime < actualFinishTime)
                return (actualFinishTime - timeSlot.FinishTime).TotalMinutes;
            
            if(scheduleStartTime < actualFinishTime)
                return -(timeSlot.FinishTime - actualFinishTime).TotalMinutes;

            return -(timeSlot.FinishTime - scheduleStartTime).TotalMinutes;

        }

        private async Task changeSlotStatus(TimeSlotInSchedule? timeSlot, TimeSlotStatus newStatus)
        {
            timeSlot.Status = newStatus;
            await _scheduleRepository.UpdateTimeSlot(timeSlot);
        }
    }
}
