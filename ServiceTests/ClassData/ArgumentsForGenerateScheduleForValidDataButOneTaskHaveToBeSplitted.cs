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
    public class ArgumentsForGenerateScheduleForValidDataButOneTaskHaveToBeSplitted : IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {



            var testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 0),
                finishTime = new TimeOnly(20, 0),
                MaxWorkTimeBeforeBreakMin=60,
                MinWorkTimeBeforeBreakMin=45
            };
            var task1 = new SingleTask("test1", 70);
            var task2 = new SingleTask("test2", 30);
            var task3 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),
                task3.Copy()
            };

            
            var expectedStartTime = new TimeOnly(12, 00);
            var expectedFinishTime = new TimeOnly(13, 00);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(13, 0);
            expectedFinishTime = new TimeOnly(13, 20);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(13, 20);
            expectedFinishTime = new TimeOnly(13, 30);
            var expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();


            expectedStartTime = new TimeOnly(13, 30);
            expectedFinishTime = new TimeOnly(14, 0);
            var expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            expectedStartTime = new TimeOnly(14, 0);
            expectedFinishTime = new TimeOnly(14, 20);
            var expectedTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();


            expectedStartTime = new TimeOnly(14, 20);
            expectedFinishTime = new TimeOnly(14, 40);
            var expectedTimeSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(14, 40);
            expectedFinishTime = new TimeOnly(14, 50);
            var expectedTimeSlot7 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4, expectedTimeSlot5, expectedTimeSlot6, expectedTimeSlot7
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };




        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
