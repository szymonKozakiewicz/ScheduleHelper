using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.Helpers.Dto;
using ScheduleHelper.ServiceTests.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;

namespace ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot
{
    public class ArgumentsFinaliseTimeSlot_NotEnoughTimeForSomeSlots : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(16, 30);
            var scheduleStartTime = new TimeOnly(12, 24);
            int finishedIndex = 2;

            ScheduleSettings scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);
            
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            SingleTask task3 = new SingleTask()
            {
                Name = "task3",
                HasStartTime = false,
                TimeMin = 40
            };
            TimeSlotsList initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            TimeSlotInSchedule timeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task=task3

            };
            initTimeSlotsList.Add(timeSlot6);

            TimeSlotsList timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            TimeSlotsList timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            var expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 4),
                FinishTime = new TimeOnly(14, 34),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            var expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(14, 54),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            var expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 54),
                FinishTime = new TimeOnly(15, 24),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };

            var expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 24),
                FinishTime = new TimeOnly(15, 50),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[1]
            };

            var expectedTimeSlot5 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 50),
                FinishTime = new TimeOnly(16, 10),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };
            var expectedTimeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(16, 10),
                FinishTime = new TimeOnly(16, 30),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = task3
            };
            var expectedTimeSlot7 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 20),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = task3
            };
            TimeSlotsList expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4,expectedTimeSlot5,expectedTimeSlot6,expectedTimeSlot7

            };
            var actualFinishTime = new TimeOnly(14, 4);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlotsList[finishedIndex].Id

            };
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            DaySchedule daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = initTimeSlotsList[finishedIndex].getDurationOfSlotInMin()
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule };


            scheduleFinishTime = new TimeOnly(16, 10);
            scheduleStartTime = new TimeOnly(12, 24);
            finishedIndex = 2;

            scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);

            listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            task3 = new SingleTask()
            {
                Name = "task3",
                HasStartTime = false,
                TimeMin = 40
            };
            initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            timeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = task3

            };
            initTimeSlotsList.Add(timeSlot6);

            timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 4),
                FinishTime = new TimeOnly(14, 34),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(14, 54),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 54),
                FinishTime = new TimeOnly(15, 24),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };

            expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 24),
                FinishTime = new TimeOnly(15, 50),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[1]
            };

            expectedTimeSlot5 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 50),
                FinishTime = new TimeOnly(16, 10),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            expectedTimeSlot7 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = task3
            };
            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4,expectedTimeSlot5,expectedTimeSlot7

            };
            actualFinishTime = new TimeOnly(14, 4);
            model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlotsList[finishedIndex].Id

            };
            finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = initTimeSlotsList[finishedIndex].getDurationOfSlotInMin()
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule };


            scheduleFinishTime = new TimeOnly(16, 5);
            scheduleStartTime = new TimeOnly(12, 24);
            finishedIndex = 2;

            scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);

            listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            task3 = new SingleTask()
            {
                Name = "task3",
                HasStartTime = false,
                TimeMin = 40
            };
            initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            timeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = task3

            };
            initTimeSlotsList.Add(timeSlot6);

            timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 4),
                FinishTime = new TimeOnly(14, 34),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(14, 54),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 54),
                FinishTime = new TimeOnly(15, 24),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };

            expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 24),
                FinishTime = new TimeOnly(15, 50),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[1]
            };

            expectedTimeSlot5 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 50),
                FinishTime = new TimeOnly(16, 5),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            expectedTimeSlot7 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = task3
            };
            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4,expectedTimeSlot5,expectedTimeSlot7

            };
            actualFinishTime = new TimeOnly(14, 4);
            model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlotsList[finishedIndex].Id

            };
            finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = initTimeSlotsList[finishedIndex].getDurationOfSlotInMin()
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule };


            scheduleFinishTime = new TimeOnly(15,50);
            scheduleStartTime = new TimeOnly(12, 24);
            finishedIndex = 2;

            scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);

            listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            task3 = new SingleTask()
            {
                Name = "task3",
                HasStartTime = false,
                TimeMin = 40
            };
            initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            timeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = task3

            };
            initTimeSlotsList.Add(timeSlot6);

            timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 4),
                FinishTime = new TimeOnly(14, 34),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(14, 54),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 54),
                FinishTime = new TimeOnly(15, 24),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };

            expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 24),
                FinishTime = new TimeOnly(15, 50),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[1]
            };



            expectedTimeSlot7 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = task3
            };
            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4,expectedTimeSlot7

            };
            actualFinishTime = new TimeOnly(14, 4);
            model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlotsList[finishedIndex].Id

            };
            finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = initTimeSlotsList[finishedIndex].getDurationOfSlotInMin()
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule };




            scheduleFinishTime = new TimeOnly(15, 30);
            scheduleStartTime = new TimeOnly(12, 24);
            finishedIndex = 2;

            scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);

            listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            task3 = new SingleTask()
            {
                Name = "task3",
                HasStartTime = false,
                TimeMin = 40
            };
            initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            timeSlot6 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = task3

            };
            initTimeSlotsList.Add(timeSlot6);

            timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 4),
                FinishTime = new TimeOnly(14, 34),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(14, 54),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 54),
                FinishTime = new TimeOnly(15, 24),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };

            expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 24),
                FinishTime = new TimeOnly(15, 30),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[1]
            };

            expectedTimeSlot5 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(14, 54),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[1]
            };


            expectedTimeSlot7 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(15, 0),
                FinishTime = new TimeOnly(15, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = task3
            };
            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4,expectedTimeSlot5,expectedTimeSlot7

            };
            actualFinishTime = new TimeOnly(14, 4);
            model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlotsList[finishedIndex].Id

            };
            finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ActualFinishTime = actualFinishTime,
                ListOfExpectedSlots = expectedTimeSlotsList,
                ListOfSlots = initTimeSlotsList,
                Model = model
            };
            daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = initTimeSlotsList[finishedIndex].getDurationOfSlotInMin()
            };
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
