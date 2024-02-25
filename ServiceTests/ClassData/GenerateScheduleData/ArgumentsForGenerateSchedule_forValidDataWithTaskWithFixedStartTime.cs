using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.ClassData.GenerateScheduleData
{
    public class ArgumentsForGenerateSchedule_forValidDataWithTaskWithFixedStartTime : IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {


            var task4 = new SingleTask()
            {
                Name = "test4",
                StartTime = new TimeOnly(18, 0),
                HasStartTime = true,
                TimeMin = 40

            };
            var testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 0),
                finishTime = new TimeOnly(20, 0),
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin = 45
            };
            var task1 = new SingleTask("test1", 70);
            var task2 = new SingleTask("test2", 30);
            var task3 = new SingleTask("test3", 30);

            List<SingleTask> tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),
                task3.Copy(),
                task4.Copy()
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

            expectedStartTime = new TimeOnly(18, 0);
            expectedFinishTime = new TimeOnly(18, 40);
            var expectedTimeSlot8 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4, expectedTimeSlot5, expectedTimeSlot6, expectedTimeSlot7
                ,expectedTimeSlot8
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };



            task4 = new SingleTask()
            {
                Name = "test4",
                StartTime = new TimeOnly(13, 40),
                HasStartTime = true,
                TimeMin = 40

            };
            tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),
                task3.Copy(),
                task4.Copy()
            };


            expectedStartTime = new TimeOnly(12, 00);
            expectedFinishTime = new TimeOnly(13, 00);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(13, 0);
            expectedFinishTime = new TimeOnly(13, 20);
            expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(13, 20);
            expectedFinishTime = new TimeOnly(13, 30);
            expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();


            expectedStartTime = new TimeOnly(13, 30);
            expectedFinishTime = new TimeOnly(13, 40);
            expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            expectedStartTime = new TimeOnly(13, 40);
            expectedFinishTime = new TimeOnly(14, 20);
            expectedTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();

            expectedStartTime = new TimeOnly(14, 20);
            expectedFinishTime = new TimeOnly(14, 40);
            expectedTimeSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(14, 40);
            expectedFinishTime = new TimeOnly(15, 0);
            expectedTimeSlot7 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();


            expectedStartTime = new TimeOnly(15, 0);
            expectedFinishTime = new TimeOnly(15, 30);
            expectedTimeSlot8 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();






            expectedTimeSlotsList = new()
            {
                expectedTimeSlot5,expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4, expectedTimeSlot6, expectedTimeSlot7
                ,expectedTimeSlot8
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };


            task4 = new SingleTask()
            {
                Name = "test4",
                StartTime = new TimeOnly(13, 40),
                HasStartTime = true,
                TimeMin = 80

            };
            tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),
                task3.Copy(),
                task4.Copy()
            };


            expectedStartTime = new TimeOnly(12, 00);
            expectedFinishTime = new TimeOnly(13, 00);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(13, 0);
            expectedFinishTime = new TimeOnly(13, 20);
            expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(13, 20);
            expectedFinishTime = new TimeOnly(13, 30);
            expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();


            expectedStartTime = new TimeOnly(13, 30);
            expectedFinishTime = new TimeOnly(13, 40);
            expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            expectedStartTime = new TimeOnly(13, 40);
            expectedFinishTime = new TimeOnly(15, 0);
            expectedTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();

            expectedStartTime = new TimeOnly(15, 0);
            expectedFinishTime = new TimeOnly(15, 20);
            expectedTimeSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();






            expectedStartTime = new TimeOnly(15, 20);
            expectedFinishTime = new TimeOnly(15, 40);
            expectedTimeSlot7 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();


            expectedStartTime = new TimeOnly(15, 40);
            expectedFinishTime = new TimeOnly(16, 10);
            expectedTimeSlot8 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();







            expectedTimeSlotsList = new()
            {
                expectedTimeSlot5, expectedTimeSlot6, expectedTimeSlot7,expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4,
                expectedTimeSlot8
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };


            task1 = new SingleTask("test1", 60);
            task4 = new SingleTask()
            {
                Name = "test4",
                StartTime = new TimeOnly(13, 10),
                HasStartTime = true,
                TimeMin = 60

            };
            tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),

                task4.Copy()
            };


            expectedStartTime = new TimeOnly(12, 00);
            expectedFinishTime = new TimeOnly(13, 00);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(13, 0);
            expectedFinishTime = new TimeOnly(13, 10);
            expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(13, 10);
            expectedFinishTime = new TimeOnly(14, 10);
            expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();


            expectedStartTime = new TimeOnly(14, 10);
            expectedFinishTime = new TimeOnly(14, 30);
            expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(14, 30);
            expectedFinishTime = new TimeOnly(15, 0);
            expectedTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();




            expectedTimeSlotsList = new()
            {
                expectedTimeSlot3, expectedTimeSlot1, expectedTimeSlot2,expectedTimeSlot4, expectedTimeSlot5
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };



            task1 = new SingleTask("test1", 60);
            task4 = new SingleTask()
            {
                Name = "test4",
                StartTime = new TimeOnly(13, 0),
                HasStartTime = true,
                TimeMin = 60

            };
            tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),

                task4.Copy()
            };

            expectedStartTime = new TimeOnly(12, 00);
            expectedFinishTime = new TimeOnly(13, 00);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(13, 0);
            expectedFinishTime = new TimeOnly(14, 0);
            expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();
            expectedStartTime = new TimeOnly(14, 0);
            expectedFinishTime = new TimeOnly(14, 20);
            expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();


            expectedStartTime = new TimeOnly(14, 20);
            expectedFinishTime = new TimeOnly(14, 50);
            expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();






            expectedTimeSlotsList = new()
            {
                expectedTimeSlot2, expectedTimeSlot1,expectedTimeSlot3, expectedTimeSlot4
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };


        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
