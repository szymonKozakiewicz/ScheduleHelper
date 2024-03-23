using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.Helpers.Dto;
using ScheduleHelper.ServiceTests.Helpers;
using System.Collections;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;

namespace ScheduleHelper.ServiceTests.ClassData
{
    public class ArgumentsCalculateAvaiableFreeTimeBasedOnExistingTasks_ExpectToReturnExpectedTime : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(21, 0);
            var scheduleStartTime = new TimeOnly(12, 0);


            ScheduleSettings scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);
            SingleTask singleTask1 = new SingleTask()
            {
                HasStartTime = false,
                Id = Guid.NewGuid(),
                Name = "task1",
                TimeMin = 60
            };
            SingleTask singleTask2 = new SingleTask()
            {
                HasStartTime = false,
                Id = Guid.NewGuid(),
                Name = "task2",
                TimeMin = 40
            };

            SingleTask singleTask3 = new SingleTask()
            {
                HasStartTime = false,
                Id = Guid.NewGuid(),
                Name = "task3",
                TimeMin = 80
            };

            TimeSlotInSchedule timeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = scheduleStartTime,
                FinishTime = new TimeOnly(13, 0),
                task = singleTask1,
                Id = Guid.NewGuid(),
                IsItBreak=false
            };
            TimeSlotInSchedule timeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(13, 0),
                FinishTime = new TimeOnly(13, 20),
                task = null,
                Id = Guid.NewGuid(),
                IsItBreak = true
            };
            TimeSlotInSchedule timeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(13, 20),
                FinishTime = new TimeOnly(14, 0),
                task = singleTask2,
                Id = Guid.NewGuid(),
                IsItBreak = false
            };

            TimeSlotInSchedule timeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 0),
                FinishTime = new TimeOnly(14, 20),
                task = singleTask3,
                Id = Guid.NewGuid(),
                IsItBreak = false
            };
            TimeSlotInSchedule timeSlot5 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 20),
                FinishTime = new TimeOnly(14, 40),
                task = null,
                Id = Guid.NewGuid(),
                IsItBreak = true
            };
            TimeSlotInSchedule timeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 40),
                FinishTime = new TimeOnly(15, 40),
                task =singleTask3,
                Id = Guid.NewGuid(),
                IsItBreak = false
            };
            TimeSlotList expectedSlots = new TimeSlotList()
            {
                timeSlot1, timeSlot2, timeSlot3, timeSlot4, timeSlot5, timeSlot6
            };

            List<SingleTask>listOfTasks=new List<SingleTask>() { singleTask1,singleTask2,singleTask3 };
            double expectedFreeTimeLeft = 320;

            yield return new object[] { listOfTasks, expectedFreeTimeLeft, scheduleSettings,expectedSlots};


        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

