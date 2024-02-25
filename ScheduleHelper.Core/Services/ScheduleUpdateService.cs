using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;


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

            await updateSlotsAfterOneSlotFinished(timeSlot, actualFinishTime);

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

        private async Task updateSlotsAfterOneSlotFinished(TimeSlotInSchedule? finishedTimeSlot, TimeOnly actualFinishTime)
        {

            var scheduleSettings = await _scheduleRepository.GetScheduleSettings();

           

            await cancelFixedSlotsForWhichThereIsNoTime(actualFinishTime);

            await removeAllBreaks();

            await readjustingSlotsAfterOneSlotFinished(actualFinishTime, scheduleSettings);


        }

        protected async Task readjustingSlotsAfterOneSlotFinished(TimeOnly actualFinishTime, ScheduleSettings scheduleSettings)
        {
            TimeSlotsList activeSlots = await getActiveTimeSlotsSortedWithStartTime();
            await loopWhichBuildScheduleByAdjustingSlots(actualFinishTime, scheduleSettings, activeSlots);
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

                    changeSlotStatus(fixedSlot, TimeSlotStatus.Canceled);
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
    }
}
