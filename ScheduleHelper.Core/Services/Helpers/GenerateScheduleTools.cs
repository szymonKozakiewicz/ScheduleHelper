using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public static class GenerateScheduleTools
    {
        public static bool SlotOverlapsWithExistingFixedTimeSlot(TimeOnly slotStartTime, TimeOnly slotFinishTime, List<TimeSlotInSchedule> fixedTimeSlots)
        {
            TimeSlotInSchedule tempTimeSlot = new TimeSlotInSchedule()
            {
                StartTime = slotStartTime,
                FinishTime = slotFinishTime,
            };
            foreach (var fixedTimeSlot in fixedTimeSlots)
            {
                if (fixedTimeSlot.TestedTimeSlotIsInsideOfTimeSlot(tempTimeSlot) || tempTimeSlot.TestedTimeSlotIsInsideOfTimeSlot(fixedTimeSlot))
                    return true;
            }
            return false;

        }

        public static double getDuartionBetweenTimes(TimeOnly startTime, TimeOnly finishTime)
        {
            return (finishTime - startTime).TotalMinutes;
        }
        public static TimeSlotInSchedule makeTimeSlot(ScheduleSettings scheduleSettings, IterationStateForGenerateSchedule iterationState, double slotDuration, SingleTask? task)
        {




            double slotDurationMin;
            bool isItBreak = task == null;
            if (isItBreak)
            {
                slotDurationMin = scheduleSettings.breakDurationMin;
            }
            else
            {
                slotDurationMin = slotDuration;
            }

            TimeOnly slotStartTime = iterationState.CurrentTime;



            if (!isItBreak && task.HasStartTime)
            {
                slotStartTime = task.StartTime;
                if(task.StartTime< iterationState.CurrentTime || task.StartTime>scheduleSettings.FinishTime)
                {
                    return null;
                }
            }



            TimeOnly slotFinishTime = slotStartTime.AddMinutes(slotDurationMin);
            var newTimeSlot = new TimeSlotInScheduleBuilder()
                   .SetFinishTime(slotFinishTime)
                   .SetStartTime(slotStartTime)
                   .SetTimeSlotStatus(TimeSlotStatus.Active)
                   .SetTask(task)
                   .SetIsItBreak(isItBreak)
                   .SetOrdinalNumber(0)
                   .Build();
            newTimeSlot= adjustFinishAndStartTime(scheduleSettings, iterationState, newTimeSlot);

            return newTimeSlot;
        }

        private static TimeSlotInSchedule adjustFinishAndStartTime(ScheduleSettings scheduleSettings, IterationStateForGenerateSchedule iterationState, TimeSlotInSchedule newTimeSlot)
        {
            double timeSlotTargetDuration = newTimeSlot.getDurationOfSlotInMin();
            adjustStartAndFinishToStartAndFinishOfSchedule(scheduleSettings, newTimeSlot);

            while (SlotOverlapsWithExistingFixedTimeSlot(newTimeSlot.StartTime, newTimeSlot.FinishTime, iterationState.FixedTimeSlots))
            {
                newTimeSlot = adjustSlotToFixOverlapsing(iterationState, newTimeSlot, timeSlotTargetDuration);

                adjustStartAndFinishToStartAndFinishOfSchedule(scheduleSettings, newTimeSlot);
                if (newTimeSlot.getDurationOfSlotInMin() < 0.1)
                    return null;

            }


            if (newTimeSlot.getDurationOfSlotInMin() < 0.1)
                return null;
            return newTimeSlot;
        }



        private static TimeSlotInSchedule adjustSlotToFixOverlapsing(IterationStateForGenerateSchedule iterationState, TimeSlotInSchedule newTimeSlot, double timeSlotTargetDuration)
        {
            var slotCopy = newTimeSlot.Copy();
            TimeSlotInSchedule OverlapsingFixedTimeSlot = GetOverlapsingFixedTimeSlot(newTimeSlot.StartTime, newTimeSlot.FinishTime, iterationState.FixedTimeSlots);
            if (OverlapsingFixedTimeSlot.StartTime <= newTimeSlot.StartTime)
            {
                slotCopy.StartTime = OverlapsingFixedTimeSlot.FinishTime;
                slotCopy.FinishTime = slotCopy.StartTime.AddMinutes(timeSlotTargetDuration);
            }
            else
                slotCopy.FinishTime = OverlapsingFixedTimeSlot.StartTime;
            return slotCopy;
        }

        private static void adjustStartAndFinishToStartAndFinishOfSchedule(ScheduleSettings scheduleSettings, TimeSlotInSchedule newTimeSlot)
        {
            if (newTimeSlot.FinishTime > scheduleSettings.FinishTime)
            {
                newTimeSlot.FinishTime = scheduleSettings.FinishTime;
            }
            if (newTimeSlot.StartTime < scheduleSettings.StartTime)
            {
                newTimeSlot.StartTime = scheduleSettings.StartTime;
            }
        }

        public static TimeSlotInSchedule GetOverlapsingFixedTimeSlot(TimeOnly slotStartTime, TimeOnly slotFinishTime, List<TimeSlotInSchedule> fixedTimeSlots)
        {

            foreach (var fixedTimeSlot in fixedTimeSlots)
            {
                if (SlotOverlapsWithExistingFixedTimeSlot(slotStartTime, slotFinishTime, fixedTimeSlots))
                    return fixedTimeSlot;
            }
            return null;
        }

        public static TimeSlotInSchedule GetLastOverlapsingFixedTimeSlot(List<TimeSlotInSchedule> fixedTimeSlots, TimeOnly finishTimeOfLastTask)
        {
            TimeOnly overlapsingFinish = finishTimeOfLastTask.AddMinutes(1);
            TimeSlotInSchedule overlapsingFixedTimeSlot = GetOverlapsingFixedTimeSlot(finishTimeOfLastTask, overlapsingFinish, fixedTimeSlots);
            List<TimeSlotInSchedule> overlapsingFixedTimeSlots = fixedTimeSlots.FindAll(slot => !slot.IsItBreak && slot.task.Id == overlapsingFixedTimeSlot.task.Id).ToList();
            TimeSlotInSchedule lastOverlapsingTimeSlot = overlapsingFixedTimeSlots[0];
            foreach (var slot in overlapsingFixedTimeSlots)
            {
                if (slot.FinishTime > lastOverlapsingTimeSlot.FinishTime)
                    lastOverlapsingTimeSlot = slot;
            }

            return lastOverlapsingTimeSlot;
        }
    }
}
