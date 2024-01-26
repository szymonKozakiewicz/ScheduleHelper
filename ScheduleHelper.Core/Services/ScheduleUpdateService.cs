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
            bool isTimeSlotIdExistInDb = timeSlot == null;
            if (isTimeSlotIdExistInDb)
            {
                throw new ArgumentException("time slot with such id isn't present on list");
            }
            await changeSlotStatus(timeSlot,TimeSlotStatus.Finished);


            TimeOnly actualFinishTime = TimeOnly.Parse(model.FinishTime);
            bool isTimeSlotFinishedOnTime = timeSlot.FinishTime.AreTimesEqualWithTolerance(actualFinishTime);
            if (isTimeSlotFinishedOnTime)
            {
                return;
            }

            var scheduleSettings=await _scheduleRepository.GetScheduleSettings();

            bool timeSlotFinishedTooLate = timeSlot.FinishTime < actualFinishTime;

            if (timeSlotFinishedTooLate)
            {
                await updateSlotsTimesWithDelay(model, timeSlot, actualFinishTime, scheduleSettings);
            }


        }

        private async Task updateSlotsTimesWithDelay(FinaliseSlotDTO model, TimeSlotInSchedule? timeSlot, TimeOnly actualFinishTime, ScheduleSettings scheduleSettings)
        {
            double dely = (actualFinishTime - timeSlot.FinishTime).TotalMinutes;
            List<TimeSlotInSchedule> activeSlots = await _scheduleRepository.GetActiveSlots();
            foreach (var slot in activeSlots)
            {
                if (slot.Id == model.SlotId)
                {
                    continue;
                }

                bool slotShouldBeFinishedInNextDay = (new TimeOnly(23, 59) - slot.FinishTime).TotalMinutes < dely;
                bool notEnoughTimeToFinishSlot = slot.FinishTime.AddMinutes(dely) > scheduleSettings.FinishTime || slotShouldBeFinishedInNextDay;
                if (notEnoughTimeToFinishSlot)
                {
                    await changeSlotStatus(slot, TimeSlotStatus.Canceled);
                    continue;
                }

                slot.StartTime=slot.StartTime.AddMinutes(dely);
                slot.FinishTime=slot.FinishTime.AddMinutes(dely);
                await _scheduleRepository.UpdateTimeSlot(slot);

            }
        }

        private async Task changeSlotStatus(TimeSlotInSchedule? timeSlot, TimeSlotStatus newStatus)
        {
            timeSlot.Status = newStatus;
            await _scheduleRepository.UpdateTimeSlot(timeSlot);
        }
    }
}
