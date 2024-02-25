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
using ScheduleHelper.Core.Domain.Entities.Enums;

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
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin = 45
            };
            var task1 = new SingleTask("test1", 60);
            var task2 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2
            };
            var expectedStartTime = new TimeOnly(12, 24);
            var expectedFinishTime = new TimeOnly(13, 24);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(13, 24);
            expectedFinishTime = new TimeOnly(13, 44);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(13, 44);
            expectedFinishTime = new TimeOnly(14, 0);
            var expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();
            expectedStartTime = new TimeOnly(12, 24);
            expectedFinishTime = new TimeOnly(12, 38);
            var expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Canceled)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2,expectedTimeSlot3,expectedTimeSlot4
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
            expectedStartTime = new TimeOnly(12, 24);
            expectedFinishTime = new TimeOnly(13, 24);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();
            expectedStartTime = new TimeOnly(13, 24);
            expectedFinishTime = new TimeOnly(13, 30);
            expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(12, 24);
            expectedFinishTime = new TimeOnly(12, 54);
            expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Canceled)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            expectedTimeSlotsList = new()
            {
              expectedTimeSlot1,expectedTimeSlot2,expectedTimeSlot4
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
