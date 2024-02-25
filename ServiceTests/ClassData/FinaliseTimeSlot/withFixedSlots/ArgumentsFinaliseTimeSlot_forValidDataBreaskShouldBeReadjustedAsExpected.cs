using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleHelper.ServiceTests.Helpers.Dto;

namespace ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot.withFixedSlots
{
    public class ArgumentsFinaliseTimeSlot_forValidDataBreaskShouldBeReadjustedAsExpected : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(22, 0);
            var scheduleStartTime = new TimeOnly(10, 0);
            ScheduleSettings scheduleSettings = new ScheduleSettings()
            {
                StartTime = scheduleStartTime,
                FinishTime = scheduleFinishTime,
                breakDurationMin = 20,
                MaxWorkTimeBeforeBreakMin = 45,
                MinWorkTimeBeforeBreakMin = 60
            };
            var task1 = new SingleTask()
            {
                Name = "test1",
                HasStartTime = false,
                StartTime = scheduleFinishTime,
                TimeMin = 60
            };
            var task2 = new SingleTask()
            {
                Name = "test2",
                HasStartTime = false,
                StartTime = scheduleFinishTime,
                TimeMin = 30
            };
            var task3 = new SingleTask()
            {
                Name = "test3",
                HasStartTime = false,
                StartTime = scheduleFinishTime,
                TimeMin = 30
            };
            var task4 = new SingleTask()
            {
                Name = "test4",
                HasStartTime = true,
                StartTime = new TimeOnly(14, 40),
                TimeMin = 30
            };
            var task5 = new SingleTask()
            {
                Name = "test5",
                HasStartTime = false,
                StartTime = new TimeOnly(10, 0),
                TimeMin = 60
            };
            List<SingleTask> tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),
                task3.Copy(),
                task4.Copy(),
                task5.Copy()
            };

            var initStartTime = new TimeOnly(10, 20);
            var initFinishTime = new TimeOnly(11, 0);
            var initTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task5)
                .Build();

            initStartTime = new TimeOnly(12, 00);
            initFinishTime = new TimeOnly(13, 00);
            var initTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetId(Guid.NewGuid())
                .SetTask(task1)
                .Build();

            initStartTime = new TimeOnly(13, 0);
            initFinishTime = new TimeOnly(13, 30);
            var initTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();
            initStartTime = new TimeOnly(13, 30);
            initFinishTime = new TimeOnly(13, 50);
            var initTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(true)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();


            initStartTime = new TimeOnly(13, 50);
            initFinishTime = new TimeOnly(14, 5);
            var initTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();

            initStartTime = new TimeOnly(14, 5);
            initFinishTime = new TimeOnly(14, 25);
            var initTimeSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(true)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();


            initStartTime = new TimeOnly(14, 25);
            initFinishTime = new TimeOnly(14, 40);
            var initTimeSlot7 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();

            initStartTime = new TimeOnly(14, 40);
            initFinishTime = new TimeOnly(15, 10);
            var initTimeSlot8 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();




            var expectedStartTime = new TimeOnly(10, 20);
            var expectedFinishTime = new TimeOnly(11, 0);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetId((Guid)initTimeSlot7.Id)
                .SetTimeSlotStatus(TimeSlotStatus.Finished)
                .SetOrdinalNumber(1)
                .SetTask(task5)
                .Build();

            expectedStartTime = new TimeOnly(12, 0);
            expectedFinishTime = new TimeOnly(12, 20);
            var expectedAddedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedStartTime = new TimeOnly(12, 20);
            expectedFinishTime = new TimeOnly(12, 40);
            var expectedAddedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(12, 40);
            expectedFinishTime = new TimeOnly(13, 20);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetId((Guid)initTimeSlot2.Id)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();


            expectedStartTime = new TimeOnly(14, 10);
            expectedFinishTime = new TimeOnly(14, 30);
            var expectedAddedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            expectedStartTime = new TimeOnly(14, 30);
            expectedFinishTime = new TimeOnly(14, 50);
            var expectedAddedSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(14, 30);
            expectedFinishTime = new TimeOnly(14, 40);
            var expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetId((Guid)initTimeSlot3.Id)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            expectedStartTime = new TimeOnly(13, 30);
            expectedFinishTime = new TimeOnly(13, 50);
            var expectedTimeToDelete1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetId((Guid)initTimeSlot4.Id)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(15, 10);
            expectedFinishTime = new TimeOnly(15, 30);
            var expectedAddedSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();


            expectedStartTime = new TimeOnly(15, 30);
            expectedFinishTime = new TimeOnly(15, 50);
            var expectedAddedSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(15, 50);
            expectedFinishTime = new TimeOnly(16, 0);
            var expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetId((Guid)initTimeSlot5.Id)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();

            expectedStartTime = new TimeOnly(14, 40);
            expectedFinishTime = new TimeOnly(15, 10);
            var expectedTimeToDelete2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetId((Guid)initTimeSlot7.Id)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();
            expectedStartTime = new TimeOnly(14, 40);
            expectedFinishTime = new TimeOnly(15, 10);
            var expectedTimeToDelete3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetId((Guid)initTimeSlot7.Id)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();

            List<TimeSlotInSchedule> initTimeSlotsList = new()
            {
                initTimeSlot1,initTimeSlot2, initTimeSlot3, initTimeSlot4,initTimeSlot5,initTimeSlot6,initTimeSlot7,initTimeSlot8

            };

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4

            };

            List<TimeSlotInSchedule> addedTimeSlotsList = new()
            {
                expectedAddedTimeSlot1,expectedAddedTimeSlot2,expectedAddedTimeSlot3,expectedAddedSlot4,expectedAddedSlot5,expectedAddedSlot6

            };

            List<TimeSlotInSchedule> deletedTimeSlotsList = new()
            {
               expectedTimeToDelete1,expectedTimeToDelete2,expectedTimeToDelete3

            };
            var actualFinishTime = new TimeOnly(12, 0);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlot1.Id

            };
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, deletedTimeSlotsList, addedTimeSlotsList };


        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
