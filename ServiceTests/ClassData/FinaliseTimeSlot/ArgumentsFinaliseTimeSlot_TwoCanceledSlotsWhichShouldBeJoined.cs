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
    public class ArgumentsFinaliseTimeSlot_TwoCanceledSlotsWhichShouldBeJoined : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var scheduleFinishTime = new TimeOnly(23, 59);
            var scheduleStartTime = new TimeOnly(1, 20);
            int finishedIndex = 2;

            ScheduleSettings scheduleSettings = GenerateDataHelper.GetNormalValidScheduleSettings(scheduleStartTime, scheduleFinishTime);
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            TimeSlotsList initTimeSlotsList = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);

            TimeSlotInSchedule canceledTimeSlot1 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(4,0),
                FinishTime = new TimeOnly(4,45),
                IsItBreak=false,
                Id=Guid.NewGuid(),
                Status=TimeSlotStatus.Canceled,
                task = listOfTasks[0],
                
             
            };

            TimeSlotInSchedule canceledTimeSlot2 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(6, 0),
                FinishTime = new TimeOnly(7, 45),
                IsItBreak = false,
                Id = Guid.NewGuid(),
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[0],


            };
            TimeSlotInSchedule canceledTimeSlot3 = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(8, 0),
                FinishTime = new TimeOnly(9, 0),
                IsItBreak = false,
                Id = Guid.NewGuid(),
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[1],


            };
            

            TimeSlotsList timeSlotsWithNoFinished = new TimeSlotsList(initTimeSlotsList);
            timeSlotsWithNoFinished.RemoveAt(finishedIndex);
            TimeSlotsList timeSlotsWithNoBreaks = new TimeSlotsList(timeSlotsWithNoFinished);
            timeSlotsWithNoBreaks.RemoveAt(1);
            timeSlotsWithNoBreaks.RemoveAt(1);

            TimeSlotList listOfCanceled = new()
            {
                canceledTimeSlot1,canceledTimeSlot2,canceledTimeSlot3
            };
            TimeSlotInSchedule slotToUpdate = new TimeSlotInSchedule()
            {
                StartTime = new TimeOnly(4, 0),
                FinishTime = new TimeOnly(6, 30),
                IsItBreak = false,
                Status = TimeSlotStatus.Canceled,
                task = listOfTasks[0],
            };

            TimeSlotsList expectedTimeSlotsList = new()
            {
                slotToUpdate

            };
            var actualFinishTime = new TimeOnly(12, 0);
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
            yield return new object[] { finaliseMethodTestSettings, scheduleSettings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, listOfCanceled };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
