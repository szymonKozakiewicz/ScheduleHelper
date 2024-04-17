using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;
using ScheduleHelper.Core.Services.Helpers;

namespace ScheduleHelper.Core.Services
{
    public class ScheduleUpdateService:ScheduleServiceBase,IScheduleUpdateService
    {            
        
        
     

        public ScheduleUpdateService(IScheduleRepository scheduleRepository):base(scheduleRepository) 
        {


            

        }

        public async Task FinaliseTimeSlot(FinaliseSlotDTO model)
        {


            TimeOnly actualFinishTime = TimeOnly.Parse(model.FinishTime);
            TimeSlotInSchedule? timeSlot = await _scheduleRepository.GetTimeSlot(model.SlotId);
            bool isTimeSlotIdExistInDb = timeSlot != null;
            if (!isTimeSlotIdExistInDb)
            {
                throw new ArgumentException("time slot with such id isn't present on list");
            }


            await changeSlotStatus(timeSlot, TimeSlotStatus.Finished);
            await updateTimeFromLastBreak(timeSlot);

            await updateSlotsWithNewCurrentTime(actualFinishTime);

        }

        public async Task UpdateSettings(ScheduleSettingsDTO scheduleSettingsDto)
        {
            var newScheduleSettings = new ScheduleSettings(scheduleSettingsDto);
            await updateScheduleSettings(scheduleSettingsDto);
            TimeOnly currentTime = await getCurrentTime(newScheduleSettings);
            var daySchedule = await _scheduleRepository.GetDaySchedule();
            bool previousScheduleExist = daySchedule != null;
            if (previousScheduleExist)
            {
                await updateSlotsWithNewCurrentTime(currentTime);
            }

        }

        private async Task updateTimeFromLastBreak(TimeSlotInSchedule? timeSlot)
        {
            DaySchedule daySchedule = await _scheduleRepository.GetDaySchedule();
            if (timeSlot.IsItBreak)
            {

                daySchedule.TimeFromLastBreakMin = 0;
            }
            else
            {
                daySchedule.TimeFromLastBreakMin += timeSlot.getDurationOfSlotInMin();
            }
            await _scheduleRepository.UpdateDaySchedule(daySchedule);
        }

        private async Task updateSlotsWithNewCurrentTime( TimeOnly currentTime)
        {

            var scheduleSettings = await _scheduleRepository.GetScheduleSettings();

           

            await cancelFixedSlotsForWhichThereIsNoTime(currentTime);

            await removeAllBreaks();

            await readjustingSlotsAfterOneSlotFinished(currentTime, scheduleSettings);
            await joinCanceledSlotsWithSameTask();

        }

        private async Task joinCanceledSlotsWithSameTask()
        {
            TimeSlotsList canceledSlotsList= await _scheduleRepository.GetCanceledSlots();
            var groupsOfSlotsWithSameTask = canceledSlotsList.GroupBy(slot => slot.task).ToList();
            foreach(var group in  groupsOfSlotsWithSameTask)
            {
                if(group.Count()>1)
                {
                    await joinSlotsIntoOneRemoveRestUpdateDb(group);
                }
            }
            
        
        }

        private async Task joinSlotsIntoOneRemoveRestUpdateDb(IGrouping<SingleTask?, TimeSlotInSchedule>? group)
        {
            TimeSlotInSchedule joinedSlot = getJoinedSlot(group);
            foreach (var slot in group)
            {
                if (slot.Id != joinedSlot.Id)
                    await _scheduleRepository.RemoveTimeSlot(slot);
            }
            await _scheduleRepository.UpdateTimeSlot(joinedSlot);
        }

        protected async Task readjustingSlotsAfterOneSlotFinished(TimeOnly actualFinishTime, ScheduleSettings scheduleSettings)
        {
            TimeSlotsList activeSlots = await getActiveTimeSlotsSortedWithStartTime();
            await removeAllActiveNotFixedSlotsFromDb(activeSlots);
            await loopWhichBuildScheduleByAdjustingSlots(actualFinishTime, scheduleSettings, activeSlots);
        }

        private async Task removeAllActiveNotFixedSlotsFromDb(TimeSlotsList activeSlots)
        {

            foreach (var slot in activeSlots)
            {
                if (!slot.IsItBreak && !slot.task.HasStartTime)
                {
                    await _scheduleRepository.RemoveTimeSlot(slot);
                }
            }
        }

        private async Task removeAllBreaks()
        {

            TimeSlotsList activeSlots = await getActiveTimeSlotsSortedWithStartTime();
            foreach (var slot in activeSlots)
            {
                if (slot.IsItBreak)
                {
                    await _scheduleRepository.RemoveTimeSlot(slot);
                }
            }
        }





        private async Task cancelFixedSlotsForWhichThereIsNoTime(TimeOnly actualFinishTime)
        {
            TimeSlotsList activeSlots=await getActiveTimeSlotsSortedWithStartTime();
            var fixedTimeSlot=getFixedTimeSlots(activeSlots);
            foreach(var fixedSlot in fixedTimeSlot)
            {
                if(fixedSlot.StartTime<actualFinishTime)
                {

                    await changeSlotStatus(fixedSlot, TimeSlotStatus.Canceled);
                }
            }
        }

        private TimeSlotsList getFixedTimeSlots(TimeSlotsList activeSlots)
        {
         
            return activeSlots.FindAll(slot =>slot.task!=null && slot.task.HasStartTime).ToList();
        }

        private async Task changeSlotStatus(TimeSlotInSchedule? timeSlot, TimeSlotStatus newStatus)
        {
            timeSlot.Status = newStatus;
            await _scheduleRepository.UpdateTimeSlot(timeSlot);
        }



        private async Task<TimeOnly> getCurrentTime(ScheduleSettings newScheduleSettings)
        {
            TimeOnly currentTime = newScheduleSettings.StartTime;
            TimeSlotsList listOfSortedActiveSlots = await getActiveTimeSlotsSortedWithStartTime();
            if (listOfSortedActiveSlots.Count > 0)
            {
                TimeOnly earliestSlotStartTime = listOfSortedActiveSlots[0].StartTime;
                if (earliestSlotStartTime > currentTime)
                {
                    currentTime = earliestSlotStartTime;
                }
            }

            return currentTime;
        }
    }
}
