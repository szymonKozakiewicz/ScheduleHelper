using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IterationState = ScheduleHelper.Core.Services.Helpers.IterationStateForGenerateSchedule;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;
using static ScheduleHelper.Core.Services.Helpers.GenerateScheduleTools;
using ScheduleHelper.Core.Domain.Entities.Helpers;

namespace ScheduleHelper.Core.Services
{
    public class ScheduleServiceBase
    {

        protected IScheduleRepository _scheduleRepository;

        public ScheduleServiceBase(IScheduleRepository scheduleRepository)
        {


            _scheduleRepository = scheduleRepository;

        }


        protected async Task loopWhichBuildScheduleByAdjustingSlots(TimeOnly actualFinishTime, ScheduleSettings scheduleSettings, TimeSlotsList slotsList)
        {
            TimeSlotsList listOfFixedTimeSlots = getFixedTimeSlots(slotsList);
            TimeSlotsList listOfSlotsWithOneSlotPerOneTask = await joinSlotsWithSameTasksAndRomevePreviousSlotsFromDb(slotsList);

            DaySchedule daySchedule = await _scheduleRepository.GetDaySchedule();



            TimeOnly currentTime = getFirstAvaiableStartTime(scheduleSettings, actualFinishTime);
            IterationState iterationState = new IterationState(currentTime, daySchedule.TimeFromLastBreakMin, listOfFixedTimeSlots);

            bool noTimeForRestSlots = false;
            foreach (var slot in listOfSlotsWithOneSlotPerOneTask)
            {
                TimeSlotInSchedule newTimeSlot = null;
                if (slot.isFixed())
                {
                    continue;
                }
                if (noTimeForRestSlots)
                {
                    await cancelSlot(slot);
                    continue;
                }
                double timeOfSlotToAssign = slot.getDurationOfSlotInMin();
                do
                {
                    await addBreakIfItShouldBeAdded(scheduleSettings,  iterationState, listOfFixedTimeSlots);
                    double duration = getSlotDuration(timeOfSlotToAssign, scheduleSettings, iterationState);
                    if (duration < 0.1)
                    {
                        slot.FinishTime = slot.StartTime.AddMinutes(timeOfSlotToAssign);
                        await cancelSlot(slot);
                        noTimeForRestSlots = true;
                        break;
                    }
                    newTimeSlot = makeTimeSlot(scheduleSettings, iterationState, duration, slot.task);
                    if (newTimeSlot == null)
                    {
                        slot.FinishTime = slot.StartTime.AddMinutes(timeOfSlotToAssign);
                        await cancelSlot(slot);
                        noTimeForRestSlots = true;
                        break;
                    }
                    await _scheduleRepository.AddNewTimeSlot(newTimeSlot);
                    iterationState.UpdateState(newTimeSlot);
                    timeOfSlotToAssign -= newTimeSlot.getDurationOfSlotInMin();
                    
                } while (timeOfSlotToAssign > 0.1);


            }
        }


        protected async Task<TimeSlotsList> getActiveTimeSlotsSortedWithStartTime()
        {
            TimeSlotsList activeSlots = await _scheduleRepository.GetActiveSlots();
            activeSlots = activeSlots.OrderBy(p => p.StartTime).ToList();
            return activeSlots;
        }

        private double getSlotDuration(double timeForSlot, ScheduleSettings scheduleSettings, IterationState iterationState)
        {
            double maxSlotDuration = scheduleSettings.MaxWorkTimeBeforeBreakMin - iterationState.TimeOfWorkFromLastBreak;
            if (maxSlotDuration < 0)
                maxSlotDuration = scheduleSettings.MaxWorkTimeBeforeBreakMin;
            var maxFinishTime = new TimeOnly(23, 59);
            double durationToMidnight = getDuartionBetweenTimes(iterationState.CurrentTime, maxFinishTime);
            maxSlotDuration = Math.Min(durationToMidnight, maxSlotDuration);
            return Math.Min(timeForSlot, maxSlotDuration);
        }

        private bool checkIfSchouldBeBreak(ScheduleSettings scheduleSettings, IterationState iterationState, TimeSlotsList fixedTimeSlots)
        {

            bool isTimeForBreak = iterationState.TimeOfWorkFromLastBreak > scheduleSettings.MinWorkTimeBeforeBreakMin;
            double halfOfBreak = scheduleSettings.breakDurationMin / 2.0;
            var fixedSlotWhichOverlapsWithBreak = GetOverlapsingFixedTimeSlot(iterationState.CurrentTime, iterationState.CurrentTime.AddMinutes(halfOfBreak), fixedTimeSlots);
            bool isSpaceForBreak = fixedSlotWhichOverlapsWithBreak == null || (fixedSlotWhichOverlapsWithBreak != null && iterationState.CurrentTime.AddMinutes(halfOfBreak) < scheduleSettings.FinishTime);

            return isTimeForBreak && isSpaceForBreak;

        }


        private async Task<TimeSlotInSchedule> makeSlotForBreak(ScheduleSettings scheduleSettings, IterationState iterationState)
        {
            var slotForBreak = makeTimeSlot(scheduleSettings, iterationState, scheduleSettings.breakDurationMin, null);

            return slotForBreak;
        }


        private async Task addBreakIfItShouldBeAdded(ScheduleSettings scheduleSettings,  IterationState iterationState, TimeSlotsList fixedSlots)
        {
            dodgeFixedSlotIfItShouldBe(iterationState, fixedSlots);

            while (checkIfSchouldBeBreak(scheduleSettings, iterationState, iterationState.FixedTimeSlots))
            {
                var slotForBreak = await makeSlotForBreak(scheduleSettings, iterationState);
                if (slotForBreak != null)
                {
                    await _scheduleRepository.AddNewTimeSlot(slotForBreak);
                    iterationState.UpdateState(slotForBreak);
                }
                else
                    break;
                dodgeFixedSlotIfItShouldBe(iterationState, fixedSlots);
            }
        }

        private static void dodgeFixedSlotIfItShouldBe(IterationState iterationState, TimeSlotsList fixedSlots)
        {
            
            
            var timeMinuteLaterThanCurrent = iterationState.CurrentTime.AddMinutes(1);
            while (SlotOverlapsWithExistingFixedTimeSlot(iterationState.CurrentTime, iterationState.CurrentTime.AddMinutes(1), fixedSlots))
            {
                if (isOneMinuteToMidNight(iterationState))
                    return;
                var fixedTimeSlot = GetOverlapsingFixedTimeSlot(iterationState.CurrentTime, timeMinuteLaterThanCurrent, fixedSlots);
                iterationState.UpdateState(fixedTimeSlot);
            }
        }

        public static bool isOneMinuteToMidNight(IterationState iterationState)
        {
            return iterationState.CurrentTime.AreTimesEqualWithTolerance(new TimeOnly(23, 59));
        }

        private async Task<TimeSlotsList> joinSlotsWithSameTasksAndRomevePreviousSlotsFromDb(TimeSlotsList activeSlots)
        {
            var slotsGroupedByTasks = activeSlots.GroupBy(slot => slot.task);
            TimeSlotsList oneSlotPerTaskList = new TimeSlotsList();
            foreach (var groupOfSlots in slotsGroupedByTasks)
            {
                if (groupOfSlots.Key.HasStartTime)
                    continue;
                TimeSlotInSchedule joinedSlot = getJoinedSlot(groupOfSlots);

                oneSlotPerTaskList.Add(joinedSlot);

            }
            return oneSlotPerTaskList;
        }


        private async Task removeSlotsFromDb(TimeSlotsList timeSlotInSchedules)
        {
            foreach (var slot in timeSlotInSchedules)
            {

                await _scheduleRepository.RemoveTimeSlot(slot);

            }
        }
        protected TimeSlotInSchedule getJoinedSlot(IGrouping<SingleTask?, TimeSlotInSchedule> groupOfSlots)
        {
            double taskDuration = 0;
            foreach (var slot in groupOfSlots)
            {
                taskDuration += slot.getDurationOfSlotInMin();
            }

            TimeSlotInSchedule slotWhichRemains = groupOfSlots.First();
            slotWhichRemains.FinishTime = slotWhichRemains.StartTime.AddMinutes(taskDuration);


            return slotWhichRemains;
        }


        private TimeSlotsList getFixedTimeSlots(TimeSlotsList activeSlots)
        {

            return activeSlots.FindAll(slot => slot.task != null && slot.task.HasStartTime).ToList();
        }
        private TimeOnly getFirstAvaiableStartTime(ScheduleSettings scheduleSettings, TimeOnly actualFinishTime)
        {
            if (scheduleSettings.StartTime < actualFinishTime)
                return actualFinishTime;
            else
                return scheduleSettings.StartTime;
        }
        private async Task cancelSlot(TimeSlotInSchedule slot)
        {
            slot.Status = TimeSlotStatus.Canceled;
            await _scheduleRepository.AddNewTimeSlot(slot);
        }

    }
}
