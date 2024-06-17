using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.Helpers.Dto;
using ScheduleHelper.ServiceTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;
using System.Collections;

namespace ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot
{
    public class ArgumentsTestforValidIdAndSlotFinishedAfterTimeAndOtherSlotsShouldBeAfterMidnight : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(23, 59);
            var scheduleStartTime = new TimeOnly(12, 20);
            int finishedIndex = 2;

            ScheduleSettings scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            TimeSlotsList initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);



            TimeSlotsList timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            TimeSlotsList timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            var expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(23, 0),
                FinishTime = new TimeOnly(23, 30),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            var expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(23, 30),
                FinishTime = new TimeOnly(23, 50),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };

            var expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(23, 50),
                FinishTime = new TimeOnly(23, 59),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };

            var expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(12, 24),
                FinishTime = new TimeOnly(12, 45),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[0]
            };

            var expectedTimeSlot5 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 34),
                FinishTime = new TimeOnly(15, 0),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[1]
            };

            TimeSlotsList expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3,expectedTimeSlot4,expectedTimeSlot5

            };
            var actualFinishTime = new TimeOnly(23, 0);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)initTimeSlotsList[finishedIndex].Id,
                ComplitedShareOfTask = 100

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
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
