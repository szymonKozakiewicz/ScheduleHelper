using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSlotsList = System.Collections.Generic.List<ScheduleHelper.Core.Domain.Entities.TimeSlotInSchedule>;
using ScheduleHelper.ServiceTests.Helpers.Dto;

namespace ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot
{
    public class ArgumentsFinaliseTimeSlot_forTimeSlotWhichIsNotFirstOnListSlotfinishedTooEarly : IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(21, 0);
            var scheduleStartTime = new TimeOnly(12, 24);
            int finishedIndex = 2;
            
            ScheduleSettings scheduleSettings =GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime,scheduleFinishTime);    
            var listOfTasks=GenerateDataHelper.GetNormalValidListOfTasks();
            TimeSlotsList initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            
            
            
            TimeSlotsList timeSlotsWithNoFinished=new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            TimeSlotsList timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            var expectedTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(14,4),
                FinishTime = new TimeOnly(14, 34),
                IsItBreak=false,
                Status=TimeSlotStatus.Active,
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
            TimeSlotsList expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2, expectedTimeSlot3, expectedTimeSlot4

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
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings ,timeSlotsWithNoFinished,timeSlotsWithNoBreaks,daySchedule};

        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
