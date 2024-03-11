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

namespace ScheduleHelper.ServiceTests.ClassData
{
    public class ArgumentsUpdateSettings_ForValidSettings_itShouldUpdateSettingsAndUpdateSchedule : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(14, 20);
            var scheduleStartTime = new TimeOnly(12, 40);
            int finishedIndex = 2;

            ScheduleSettings scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            TimeSlotsList initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);


            


            var expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(12, 40),
                FinishTime = new TimeOnly(13, 40),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[0]
            };
            var expectedTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(13, 40),
                FinishTime = new TimeOnly(14, 0),
                IsItBreak = true,
                Status = TimeSlotStatus.Active,
                task = null
            };


            var expectedTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14, 0),
                FinishTime = new TimeOnly(14, 20),
                IsItBreak = false,
                Status = TimeSlotStatus.Active,
                task = listOfTasks[1]
            };

            var expectedTimeSlot4 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(13, 44),
                FinishTime = new TimeOnly(14, 20),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[1]
            };
            TimeSlotsList expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2,expectedTimeSlot3, expectedTimeSlot4

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
                TimeFromLastBreakMin = 0
            };

            yield return new object[] { initTimeSlotsList,expectedTimeSlotsList, scheduleSettings, daySchedule};
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
