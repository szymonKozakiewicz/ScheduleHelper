using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.ClassData
{
    public class ArgumentsForGenerateScheduleTestWithValidDataButNotEnoughTimeForAllTasks : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {



            var testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(14, 0),
                MaxWorkTimeBeforeBreakMin = 200,
                MinWorkTimeBeforeBreakMin = 1
            };
            var task1 = new SingleTask("test1", 60);
            var task2 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2
            };
            var expectedStartTime1 = new TimeOnly(12, 24);
            var expectedFinishTime1 = new TimeOnly(13, 24);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };


            testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(13, 30),
                MaxWorkTimeBeforeBreakMin = 200,
                MinWorkTimeBeforeBreakMin = 1
            };
            task1 = new SingleTask("test1", 60);
            task2 = new SingleTask("test2", 30);
            tasksListsInMemory = new()
            {
                task1,
                task2
            };
            expectedStartTime1 = new TimeOnly(12, 24);
            expectedFinishTime1 = new TimeOnly(13, 24);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
