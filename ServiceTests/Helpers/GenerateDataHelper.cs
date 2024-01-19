using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
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
            };
        }

        public static List<TimeSlotInSchedule> GetTimeSlotsList(SingleTask task1, SingleTask task2)
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

    }
}
