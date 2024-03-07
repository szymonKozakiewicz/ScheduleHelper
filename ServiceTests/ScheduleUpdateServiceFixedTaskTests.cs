using FluentAssertions;
using Moq;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot.withFixedSlots;
using ScheduleHelper.ServiceTests.Helpers.Dto;

namespace ScheduleHelper.ServiceTests
{

    public class ScheduleUpdateServiceFixedTaskTests:ScheduleUpdateServiceTestsBase
    {
        private IScheduleUpdateService _scheduleUpdateService;
        Mock<ITaskRespository> _taskRepositoryMock;
        Mock<IScheduleRepository> _scheduleRespositorMock;
        ITaskRespository _taskRespository;
        IScheduleRepository _scheduleRespository;

        public ScheduleUpdateServiceFixedTaskTests()
        {
            _taskRepositoryMock = new Mock<ITaskRespository>();
            _scheduleRespositorMock = new Mock<IScheduleRepository>();
            _taskRespository = _taskRepositoryMock.Object;
            _scheduleRespository = _scheduleRespositorMock.Object;
            _scheduleUpdateService = new ScheduleUpdateService(_scheduleRespository);
        }

        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_AfterTimeThereIsFixedTimeSlotInSchedule))]
        public async Task FinaliseTimeSlot_AfterTimeThereIsFixedTimeSlotInSchedule_UpdatedShouldBeExpectedListOfSlots(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings,TimeSlotList slotsWithNoBreaks)
        {
            TimeSlotList listOfAddedSlots = setupMockMethodsForFinaliseTimeSlotAfterTimeTest(finaliseMethodTestSettings, settings, slotsWithNoBreaks);
            _scheduleRespositorMock.Setup(m => m.GetCanceledSlots())
                    .ReturnsAsync(new TimeSlotList());

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert

            listOfAddedSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);


            listOfAddedSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);

        }

        private TimeSlotList setupMockMethodsForFinaliseTimeSlotAfterTimeTest(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList slotsWithNoBreaks)
        {
            DaySchedule daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = 60
            };
            TimeSlotList listOfAddedSlots = new();
            TimeSlotList listOfActiveSlotsWithNoFinished = new TimeSlotList(finaliseMethodTestSettings.ListOfSlots);
            listOfActiveSlotsWithNoFinished.RemoveAt(0);
            TimeSlotList listOfActiveSlotsWithNoCanceled = new TimeSlotList(listOfActiveSlotsWithNoFinished);

            TimeSlotList listOfActiveSlotsWithNoBreaks = slotsWithNoBreaks;
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot2(finaliseMethodTestSettings, settings, 0);
            _scheduleRespositorMock.Setup(m => m.GetDaySchedule())
                .ReturnsAsync(daySchedule);
            _scheduleRespositorMock.SetupSequence(m => m.GetActiveSlots())
                .ReturnsAsync(listOfActiveSlotsWithNoFinished)
                .ReturnsAsync(listOfActiveSlotsWithNoCanceled)
                .ReturnsAsync(listOfActiveSlotsWithNoBreaks);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()))
              .Callback((TimeSlotInSchedule m) => listOfAddedSlots.Add(m));
            return listOfAddedSlots;
        }

        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_AfterTimeThereIsFixedTimeWhichShouldBeCanceled))]
        public async Task FinaliseTimeSlot_AfterTimeThereIsFixedTimeWhichStartBeforeFinishedSlot_FixedTimeSlotShouldBeCanceled(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings)
        {



            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot2(finaliseMethodTestSettings, settings, 0);
            TimeSlotList listOfAddedSlots = new();
            DaySchedule daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = 60
            };
            TimeSlotList listOfActiveSlotsWithNoFinished = new TimeSlotList(finaliseMethodTestSettings.ListOfSlots);
            listOfActiveSlotsWithNoFinished.RemoveAt(0);
            TimeSlotList listOfActiveSlotsWithNoCanceled = new TimeSlotList(listOfActiveSlotsWithNoFinished);
            listOfActiveSlotsWithNoCanceled.RemoveAt(5);
            TimeSlotList listOfActiveSlotsWithNoBreaks = new TimeSlotList(listOfActiveSlotsWithNoCanceled);
            listOfActiveSlotsWithNoBreaks.RemoveAt(0);
            listOfActiveSlotsWithNoBreaks.RemoveAt(2);
            _scheduleRespositorMock.Setup(m => m.GetDaySchedule())
                .ReturnsAsync(daySchedule);
            _scheduleRespositorMock.Setup(m => m.GetCanceledSlots())
                    .ReturnsAsync(new TimeSlotList());
            _scheduleRespositorMock.SetupSequence(m => m.GetActiveSlots())
                .ReturnsAsync(listOfActiveSlotsWithNoFinished)
                .ReturnsAsync(listOfActiveSlotsWithNoCanceled)
                .ReturnsAsync(listOfActiveSlotsWithNoBreaks);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()))
              .Callback((TimeSlotInSchedule m) => listOfAddedSlots.Add(m));



            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(2);
            listOfUpdatedTimeSlots[1].Status.Should().Be(TimeSlotStatus.Canceled);
            listOfUpdatedTimeSlots[1].Id.Should().Be(finaliseMethodTestSettings.ListOfSlots[6].Id);

            listOfAddedSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);

        }




        protected TimeSlotList setupMockMethodsForFinaliseTimeSlot2(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, int indexOfFinishedSlot)
        {

            TimeSlotList listOfUpdatedTimeSlots = new TimeSlotList();
            ScheduleSettings scheduleSettings = settings;
            
            var slotTofinish = finaliseMethodTestSettings.ListOfSlots[indexOfFinishedSlot];
            var listOfActiveSlots = new TimeSlotList(finaliseMethodTestSettings.ListOfSlots);
            listOfActiveSlots.Remove(finaliseMethodTestSettings.ListOfSlots[indexOfFinishedSlot]);
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                .Callback((TimeSlotInSchedule slot) => listOfUpdatedTimeSlots.Add(slot));
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(It.IsAny<Guid>()))
                    .ReturnsAsync(slotTofinish);
            _scheduleRespositorMock.Setup(m => m.GetScheduleSettings())
                .ReturnsAsync(scheduleSettings);
            _scheduleRespositorMock.Setup(m => m.GetActiveSlots())
                .ReturnsAsync(listOfActiveSlots);

            



            return listOfUpdatedTimeSlots;
        }

    }
}
