﻿using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.Helpers.Dto;

namespace ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot.withFixedSlots
{
    public class ArgumentsFinaliseTimeSlot_AfterTimeThereIsFixedTimeSlotInSchedule : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(22, 0);
            var scheduleStartTime = new TimeOnly(12, 0);
            ScheduleSettings scheduleSettings = new ScheduleSettings()
            {
                StartTime = scheduleStartTime,
                FinishTime = scheduleFinishTime,
                breakDurationMin = 20,
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin = 45
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
                StartTime = new TimeOnly(15, 0),
                TimeMin = 30
            };
            List<SingleTask> tasksListsInMemory = new()
            {
                task1.Copy(),
                task2.Copy(),
                task3.Copy(),
                task4.Copy()
            };

            var initStartTime = new TimeOnly(12, 00);
            var initFinishTime = new TimeOnly(13, 00);
            var initTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetId(Guid.NewGuid())
                .SetTask(task1)
                .Build();

            initStartTime = new TimeOnly(13, 0);
            initFinishTime = new TimeOnly(13, 20);
            var initTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(true)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            initStartTime = new TimeOnly(13, 20);
            initFinishTime = new TimeOnly(13, 50);
            var initTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();


            initStartTime = new TimeOnly(13, 50);
            initFinishTime = new TimeOnly(14, 20);
            var initTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();

            initStartTime = new TimeOnly(14, 20);
            initFinishTime = new TimeOnly(14, 40);
            var initTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(true)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();


            initStartTime = new TimeOnly(14, 40);
            initFinishTime = new TimeOnly(15, 10);
            var initTimeSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();













            var expectedStartTime = new TimeOnly(13, 20);
            var expectedFinishTime = new TimeOnly(13, 40);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(13, 40);
            expectedFinishTime = new TimeOnly(14, 10);
            var expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();


            expectedStartTime = new TimeOnly(14, 10);
            expectedFinishTime = new TimeOnly(14, 40);
            var expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();






            List<TimeSlotInSchedule> initTimeSlotsList = new()
            {
                initTimeSlot1,initTimeSlot2, initTimeSlot3, initTimeSlot4,initTimeSlot5,initTimeSlot6

            };

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4

            };
            List<TimeSlotInSchedule> timeSlotsWithNoBreaks = new()
            {
                 initTimeSlot3, initTimeSlot4,initTimeSlot6
            };
            var actualFinishTime = new TimeOnly(13, 20);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlot1.Id,
                ComplitedShareOfTask = 100

            };
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoBreaks };





            task4 = new SingleTask()
            {
                Name = "test4",
                HasStartTime = true,
                StartTime = new TimeOnly(14, 20),
                TimeMin = 30
            };


            initStartTime = new TimeOnly(12, 00);
            initFinishTime = new TimeOnly(13, 00);
            initTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetId(Guid.NewGuid())
                .SetTask(task1)
                .Build();

            initStartTime = new TimeOnly(13, 0);
            initFinishTime = new TimeOnly(13, 15);
            initTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();
            initStartTime = new TimeOnly(13, 15);
            initFinishTime = new TimeOnly(13, 35);
            initTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(true)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();


            initStartTime = new TimeOnly(13, 35);
            initFinishTime = new TimeOnly(13, 50);
            initTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();

            initStartTime = new TimeOnly(13, 50);
            initFinishTime = new TimeOnly(14, 20);
            initTimeSlot5 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();


            initStartTime = new TimeOnly(14, 20);
            initFinishTime = new TimeOnly(14, 50);
            initTimeSlot6 = new TimeSlotInScheduleBuilder()
                .SetStartTime(initStartTime)
                .SetFinishTime(initFinishTime)
                .SetIsItBreak(false)
                .SetId(Guid.NewGuid())
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .SetOrdinalNumber(1)
                .SetTask(task4)
                .Build();









            expectedStartTime = new TimeOnly(14, 0);
            expectedFinishTime = new TimeOnly(14, 20);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();



            expectedStartTime = new TimeOnly(14, 50);
            expectedFinishTime = new TimeOnly(15, 20);
            expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task2)
                .Build();
            expectedStartTime = new TimeOnly(15, 20);
            expectedFinishTime = new TimeOnly(15, 40);
            expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(true)
                .SetOrdinalNumber(1)
                .SetTask(null)
                .Build();
            expectedStartTime = new TimeOnly(15, 40);
            expectedFinishTime = new TimeOnly(16, 10);
            expectedTimeSlot4 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime)
                .SetFinishTime(expectedFinishTime)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task3)
                .Build();








            initTimeSlotsList = new()
            {
                initTimeSlot1,initTimeSlot2, initTimeSlot3, initTimeSlot4,initTimeSlot5,initTimeSlot6

            };

            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1, expectedTimeSlot2, expectedTimeSlot3,expectedTimeSlot4

            };
            timeSlotsWithNoBreaks = new()
            {
                initTimeSlot2, initTimeSlot4,initTimeSlot5,initTimeSlot6
            };
            actualFinishTime = new TimeOnly(14, 0);
            model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlot1.Id,
                ComplitedShareOfTask = 100

            };
            finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoBreaks };


        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
