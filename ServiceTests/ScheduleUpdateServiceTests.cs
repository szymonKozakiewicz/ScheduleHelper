using FluentAssertions;
using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.Entities.Helpers;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using ScheduleHelper.ServiceTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.ServiceTests
{
    public class ScheduleUpdateServiceTests
    {
        private IScheduleUpdateService _scheduleUpdateService;
        Mock<ITaskRespository> _taskRepositoryMock;
        Mock<IScheduleRepository> _scheduleRespositorMock;
        ITaskRespository _taskRespository;
        IScheduleRepository _scheduleRespository;

        public ScheduleUpdateServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRespository>();
            _scheduleRespositorMock = new Mock<IScheduleRepository>();
            _taskRespository = _taskRepositoryMock.Object;
            _scheduleRespository = _scheduleRespositorMock.Object;
            _scheduleUpdateService = new ScheduleUpdateService(_scheduleRespository);
        }


        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedOnTime_itShouldSetTimeSlotStatusForFinished()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsList(listOfTasks[0], listOfTasks[1]);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = listOfSlots[0].FinishTime.ToString(),
                SlotId = (Guid)listOfSlots[0].Id

            };
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(listOfSlots[0]));
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(model.SlotId))
                    .ReturnsAsync(listOfSlots[0]);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(model);

            //assert
            _scheduleRespositorMock.Verify(m => m.UpdateTimeSlot(listOfSlots[0]), Times.Once);
        }

        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTime_itShouldUpdateAll()
        {
            int delay = 40;
            var scheduleFinishTime = new TimeOnly(20, 0);

            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                bool slotWasOnExpectedList = CheckIfSlotIsOnExpectedList(delay, finaliseMethodTestSettings, updatedSlot);
                slotWasOnExpectedList.Should().BeTrue();
            }
        }



        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTimeAndAfterMidnight_itShouldUpdateAll()
        {
            int delay = 60*10+20;
            var scheduleFinishTime = new TimeOnly(23, 0);

            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime);

            foreach(var slot in finaliseMethodTestSettings.ListOfExpectedSlots)
            {
                slot.Status = TimeSlotStatus.Canceled;
            }

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[0].Id)
                    continue;

                updatedSlot.Status.Should().Be(TimeSlotStatus.Canceled);
            }
        }

        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTimeAndNoTimeForLastSlot_itShouldUpdateAll()
        {
            int delay = 60;
            var scheduleFinishTime = new TimeOnly(15, 0);

            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime);

            foreach (var slot in finaliseMethodTestSettings.ListOfExpectedSlots)
            {
                slot.Status = TimeSlotStatus.Canceled;
            }

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[0].Id)
                    continue;
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[2].Id)
                {
                    updatedSlot.Status.Should().Be(TimeSlotStatus.Canceled);
                    continue;
                }
                updatedSlot.Status.Should().Be(TimeSlotStatus.Active);
                checkIfExpectedAndUpdatedAreSame(delay, updatedSlot, finaliseMethodTestSettings.ListOfExpectedSlots[1]);

            }
        }


        [Fact]
        public async Task FinaliseTimeSlot_ForNotExistingId_itShouldRiseArgumentException()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsList(listOfTasks[0], listOfTasks[1]);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = listOfSlots[0].FinishTime.ToString(),
                SlotId = (Guid)listOfSlots[0].Id

            };
            TimeSlotInSchedule resultOfGetTimeSlot = null;
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(model.SlotId))
                .ReturnsAsync(resultOfGetTimeSlot);


            //act

            Func<Task> action = async () => await _scheduleUpdateService.FinaliseTimeSlot(model);


            //assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        private static bool CheckIfSlotIsOnExpectedList(int delay, FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, TimeSlotInSchedule updatedSlot)
        {
            bool updatedSlotIsFinished = updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[0].Id;
            if (updatedSlotIsFinished)
                return true;
            bool slotWasOnExpectedList = false;
            foreach (var expectedTimeSlot in finaliseMethodTestSettings.ListOfExpectedSlots)
            {
                bool expectedAndUpdatedSlotAreSame = checkIfExpectedAndUpdatedAreSame(delay, updatedSlot, expectedTimeSlot);
                if (expectedAndUpdatedSlotAreSame)
                {
                    slotWasOnExpectedList = true;
                    break;
                }
            }

            return slotWasOnExpectedList;
        }

        private static bool checkIfExpectedAndUpdatedAreSame(int delay, TimeSlotInSchedule updatedSlot, TimeSlotInSchedule expectedTimeSlot)
        {
            TimeOnly expectedFinishTime = expectedTimeSlot.FinishTime.AddMinutes(delay);
            TimeOnly expectedStartTime = expectedTimeSlot.StartTime.AddMinutes(delay);
            bool startTimeSame = expectedStartTime.AreTimesEqualWithTolerance(updatedSlot.StartTime);
            bool finishTimeSame = expectedFinishTime.AreTimesEqualWithTolerance(updatedSlot.FinishTime);
            bool sameStatus = expectedTimeSlot.Status == updatedSlot.Status;
            bool expectedAndUpdatedSlotAreSame = startTimeSame && finishTimeSame && sameStatus;
            return expectedAndUpdatedSlotAreSame;
        }

        private static FinaliseMethodTestSettingsDTO getFinaliseMethodTestSettings(int delay)
        {
            var listOfSlots = getSlotsList();
            var listOfExpectedSlots = getSlotsList();
            var actualFinishTime = listOfSlots[0].FinishTime.AddMinutes(delay);
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)listOfSlots[0].Id

            };
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = new FinaliseMethodTestSettingsDTO()
            {
                ListOfExpectedSlots = listOfExpectedSlots,
                ListOfSlots = listOfSlots,
                ActualFinishTime = actualFinishTime,
                Model = model
            };
            return finaliseMethodTestSettings;
        }

        private static List<TimeSlotInSchedule> getSlotsList()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsList(listOfTasks[0], listOfTasks[1]);
            return listOfSlots;
        }

        private List<TimeSlotInSchedule> setupMockMethodsForFinaliseTimeSlot(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings,TimeOnly scheduleFinishTime)
        {
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = new List<TimeSlotInSchedule>();
            ScheduleSettings scheduleSettings = new ScheduleSettings()
            {
                FinishTime = scheduleFinishTime,
                Id = 1
            };

            var listOfActiveSlots = new List<TimeSlotInSchedule>(finaliseMethodTestSettings.ListOfSlots);
            listOfActiveSlots.Remove(finaliseMethodTestSettings.ListOfSlots[0]);
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                .Callback((TimeSlotInSchedule slot) => listOfUpdatedTimeSlots.Add(slot));
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(finaliseMethodTestSettings.Model.SlotId))
                    .ReturnsAsync(finaliseMethodTestSettings.ListOfSlots[0]);
            _scheduleRespositorMock.Setup(m => m.GetScheduleSettings())
                .ReturnsAsync(scheduleSettings);
            _scheduleRespositorMock.Setup(m => m.GetActiveSlots())
                .ReturnsAsync(listOfActiveSlots);

            return listOfUpdatedTimeSlots;
        }


    }
}
