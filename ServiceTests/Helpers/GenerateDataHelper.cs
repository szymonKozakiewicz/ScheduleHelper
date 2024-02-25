using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.Helpers
{
    public static class GenerateDataHelper
    {
        public static List<SingleTask> GetNormalValidListOfTasks()
        {
            var task1 = new SingleTask("test1", 60);
            SingleTask task2 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2
            };
            return tasksListsInMemory;
        }

        public static ScheduleSettingsDTO GetNormalValidScheduleSettings()
        {
            return new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 0),
                MaxWorkTimeBeforeBreakMin = 2000,
                MinWorkTimeBeforeBreakMin = 1
            };
        }
        public static ScheduleSettings GetNormalValidScheduleSettings(TimeOnly startTime,TimeOnly finishTime)
        {
            return new ScheduleSettings()
            {
                StartTime = startTime,
                FinishTime = finishTime,
                breakDurationMin = 20,
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin = 45
            };
        }



        public static List<TimeSlotInSchedule> GetTimeSlotsListWith3TimeSlots(SingleTask task1, SingleTask task2)
        {
            var expectedStartTime1 = new TimeOnly(12, 24);
            var expectedFinishTime1 = new TimeOnly(13, 24);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();
            var expectedStartTime2 = new TimeOnly(13, 24);
            var expectedFinishTime2 = new TimeOnly(13, 44);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime2)
                .SetFinishTime(expectedFinishTime2)
                .SetId(Guid.NewGuid())
                .SetIsItBreak(true)
                .SetOrdinalNumber(2)
                .SetTask(null)
                .Build();
            var expectedStartTime3 = new TimeOnly(13, 44);
            var expectedFinishTime3 = new TimeOnly(14, 14);
            var expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime3)
                .SetFinishTime(expectedFinishTime3)
                .SetId(Guid.NewGuid())
                .SetIsItBreak(false)
                .SetOrdinalNumber(3)
                .SetTask(task2)
                .Build();
            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2,expectedTimeSlot3
            };
            return expectedTimeSlotsList;
        }


        public static List<TimeSlotInSchedule> GetTimeSlotsListWith5TimeSlots(SingleTask task1, SingleTask task2)
        {

            var timeSlotList = GetTimeSlotsListWith3TimeSlots(task1,task2);
            var expectedStartTime4 = new TimeOnly(14, 14);
            var expectedFinishTime4 = new TimeOnly(14, 34);
            var expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime4)
                .SetFinishTime(expectedFinishTime4)
                .SetId(Guid.NewGuid())
                .SetIsItBreak(true)
                .SetOrdinalNumber(4)
                .SetTask(task2)
                .Build();

            var expectedStartTime5 = new TimeOnly(14, 34);
            var expectedFinishTime5 = new TimeOnly(15, 0);
            var expectedTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime5)
                .SetFinishTime(expectedFinishTime5)
                .SetId(Guid.NewGuid())
                .SetIsItBreak(false)
                .SetOrdinalNumber(5)
                .SetTask(task2)
                .Build();
            timeSlotList.Add(expectedTimeSlot4);
            timeSlotList.Add(expectedTimeSlot5);
            return timeSlotList;

        }



    }
}
