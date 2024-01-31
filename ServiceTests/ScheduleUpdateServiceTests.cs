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
            var listOfSlots = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(listOfTasks[0], listOfTasks[1]);
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
            var scheduleStartTime = new TimeOnly(6, 0);

            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay,0);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime , 0);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                bool slotWasOnExpectedList = CheckIfSlotIsOnExpectedList(delay, finaliseMethodTestSettings, updatedSlot,0);
                slotWasOnExpectedList.Should().BeTrue();
            }
        }



        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTimeAndAfterMidnight_itShouldUpdateAll()
        {
            int delay = 60*10+20;
            var scheduleFinishTime = new TimeOnly(23, 0);
            var scheduleStartTime = new TimeOnly(6, 0);

            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay, 0);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime, 0);

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
            var scheduleStartTime = new TimeOnly(6, 0);

            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay, 0);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime,scheduleStartTime , 0);

            finaliseMethodTestSettings.ListOfExpectedSlots[2].Status=TimeSlotStatus.Canceled;

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
                bool expectedAndUpdatedAreSame =checkIfExpectedAndUpdatedAreSame(delay, updatedSlot, finaliseMethodTestSettings.ListOfExpectedSlots[1]);
                expectedAndUpdatedAreSame.Should().BeTrue();    
            }
        }


        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedTooEarlyButAfterScheduleStartTime_itShouldUpdateAll()
        {
            int delay = -60;
            var scheduleFinishTime = new TimeOnly(15, 0);
            var scheduleStartTime = new TimeOnly(6, 0);


            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay, 0);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime, 0);



            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[0].Id)
                    continue;
                updatedSlot.Status.Should().Be(TimeSlotStatus.Active);
                bool expectedAndUpdatedAreSame = false;
                foreach(var expectedSlot in finaliseMethodTestSettings.ListOfExpectedSlots)
                {
                    if( checkIfExpectedAndUpdatedAreSame(delay, updatedSlot, expectedSlot))
                    {
                        expectedAndUpdatedAreSame = true;
                    }

                }
                
                expectedAndUpdatedAreSame.Should().BeTrue();
            }
        }


        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedTooEarlyButBeforeScheduleStartTime_itShouldUpdateAll()
        {
            int delay = -300;
            var scheduleFinishTime = new TimeOnly(15, 0);
            var scheduleStartTime = new TimeOnly(11, 0);


            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettings(delay, 0);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime,  0);



            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            delay = -144;
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[0].Id)
                    continue;
                updatedSlot.Status.Should().Be(TimeSlotStatus.Active);
                bool expectedAndUpdatedAreSame = false;
                foreach (var expectedSlot in finaliseMethodTestSettings.ListOfExpectedSlots)
                {
                    if (checkIfExpectedAndUpdatedAreSame(delay, updatedSlot, expectedSlot))
                    {
                        expectedAndUpdatedAreSame = true;
                    }

                }

                expectedAndUpdatedAreSame.Should().BeTrue();
            }
        }


        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndTimeSlotWhichIsNotFirstOnListslotfinishedTooEarly_itShouldUpdateSlotsAsExpected()
        {
            int delay = -10;
            int indexOfFinalisedSlot = 2;
            var scheduleFinishTime = new TimeOnly(20, 0);
            var scheduleStartTime = new TimeOnly(6, 0);
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettingsWith5TimeSlots(delay, indexOfFinalisedSlot);
            finaliseMethodTestSettings.ListOfExpectedSlots = getExpectedTimeSlotsForTest1(finaliseMethodTestSettings);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime, indexOfFinalisedSlot);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(5);
            
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[indexOfFinalisedSlot].Id)
                {
                    updatedSlot.Status.Should().Be(TimeSlotStatus.Finished);
                    continue;
                }

                bool isOnExpectedList = CheckIfSlotIsOnExpectedList(0, finaliseMethodTestSettings, updatedSlot,indexOfFinalisedSlot);



                isOnExpectedList.Should().BeTrue();
            }
            
        }


        [Fact]
        public async Task FinaliseTimeSlot_forValidIdAndTimeSlotWhichIsNotFirstOnListSlotfinishedBeforeScheduleStarted_itShouldUpdateSlotsAsExpected()
        {
            int delay = -360;
            int indexOfFinalisedSlot = 2;
            var scheduleFinishTime = new TimeOnly(20, 0);
            var scheduleStartTime = new TimeOnly(11, 0);
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettingsWith5TimeSlots(delay, indexOfFinalisedSlot);
            finaliseMethodTestSettings.ListOfExpectedSlots = getExpectedTimeSlotsForTest2(finaliseMethodTestSettings);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime, indexOfFinalisedSlot);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(5);

            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[indexOfFinalisedSlot].Id)
                {
                    updatedSlot.Status.Should().Be(TimeSlotStatus.Finished);
                    continue;
                }

                bool isOnExpectedList = CheckIfSlotIsOnExpectedList(0, finaliseMethodTestSettings, updatedSlot, indexOfFinalisedSlot);



                isOnExpectedList.Should().BeTrue();
            }

        }

        [Theory]
        [InlineData(16,0,1)]
        [InlineData(15,40,2)]
        [InlineData(15,20,3)]
        [InlineData(14,20,4)]
        public async Task FinaliseTimeSlot_forValidIdAndTimeSlotWhichIsNotFirstOnListNotEnoughTimeForSomeSlots_itShouldHaveExpectedNumberOfCanceledSlots(int scheduleFinishHour, int scheduleFinishMinute, int numberOfCanceledSlot)
        {
            int delay = -10;
            int indexOfFinalisedSlot = 2;
            var scheduleFinishTime = new TimeOnly(scheduleFinishHour, scheduleFinishMinute);
            var scheduleStartTime = new TimeOnly(6, 0);
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseMethodTestSettingsWith5TimeSlots(delay, indexOfFinalisedSlot);
            finaliseMethodTestSettings.ListOfExpectedSlots = getExpectedTimeSlotsForTest1(finaliseMethodTestSettings);
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, scheduleFinishTime, scheduleStartTime, indexOfFinalisedSlot);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(5);
            var numberOfCanceledSlots = 0;
            foreach (var updatedSlot in listOfUpdatedTimeSlots)
            {
                if (updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[indexOfFinalisedSlot].Id)
                {
                    updatedSlot.Status.Should().Be(TimeSlotStatus.Finished);
                    continue;
                }
                if(updatedSlot.Status==TimeSlotStatus.Canceled)
                {
                    numberOfCanceledSlots++;
                    continue;
                }    
                bool isOnExpectedList = CheckIfSlotIsOnExpectedList(0, finaliseMethodTestSettings, updatedSlot, indexOfFinalisedSlot);



                isOnExpectedList.Should().BeTrue();
            }
            numberOfCanceledSlots.Should().Be(numberOfCanceledSlot);

        }


        [Fact]
        public async Task FinaliseTimeSlot_ForNotExistingId_itShouldRiseArgumentException()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(listOfTasks[0], listOfTasks[1]);
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

        private static List<TimeSlotInSchedule> getExpectedTimeSlotsForTest1(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings)
        {
            var expectedTimeSlots = finaliseMethodTestSettings.ListOfExpectedSlots;
            expectedTimeSlots.Remove(expectedTimeSlots[2]);
            
            expectedTimeSlots[0].StartTime = new TimeOnly(14, 4);
            expectedTimeSlots[0].FinishTime = new TimeOnly(14, 24);
            expectedTimeSlots[1].StartTime = new TimeOnly(14, 24);
            expectedTimeSlots[1].FinishTime = new TimeOnly(15, 24);
            expectedTimeSlots[2].StartTime = new TimeOnly(15, 24);
            expectedTimeSlots[2].FinishTime = new TimeOnly(15, 44);
            expectedTimeSlots[3].StartTime = new TimeOnly(15, 44);
            expectedTimeSlots[3].FinishTime = new TimeOnly(16, 10);
            return expectedTimeSlots;
        }

        private static List<TimeSlotInSchedule> getExpectedTimeSlotsForTest2(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings)
        {
            var expectedTimeSlots = finaliseMethodTestSettings.ListOfExpectedSlots;
            expectedTimeSlots.Remove(expectedTimeSlots[2]);

            expectedTimeSlots[0].StartTime = new TimeOnly(11, 00);
            expectedTimeSlots[0].FinishTime = new TimeOnly(11, 20);
            expectedTimeSlots[1].StartTime = new TimeOnly(11, 20);
            expectedTimeSlots[1].FinishTime = new TimeOnly(12, 20);
            expectedTimeSlots[2].StartTime = new TimeOnly(12, 20);
            expectedTimeSlots[2].FinishTime = new TimeOnly(12, 40);
            expectedTimeSlots[3].StartTime = new TimeOnly(12, 40);
            expectedTimeSlots[3].FinishTime = new TimeOnly(13, 6);
            return expectedTimeSlots;
        }
        private static bool CheckIfSlotIsOnExpectedList(int delay, FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, TimeSlotInSchedule updatedSlot,int finishedSlotIndex)
        {
            bool updatedSlotIsFinished = updatedSlot.Id == finaliseMethodTestSettings.ListOfSlots[finishedSlotIndex].Id;
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

        private static FinaliseMethodTestSettingsDTO getFinaliseMethodTestSettings(int delay,int indexSlotToFinish)
        {
            var listOfSlots = getSlotsListWith3Slots();
            var listOfExpectedSlots = getSlotsListWith3Slots();
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseTestSettingsBasedOnGivenListOfTimeSlots(delay, indexSlotToFinish, listOfSlots, listOfExpectedSlots);
            return finaliseMethodTestSettings;
        }



        private static FinaliseMethodTestSettingsDTO getFinaliseMethodTestSettingsWith5TimeSlots(int delay, int indexSlotToFinish)
        {
            var listOfSlots = getSlotsListWith5Slots();
            var listOfExpectedSlots = getSlotsListWith5Slots();
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseTestSettingsBasedOnGivenListOfTimeSlots(delay, indexSlotToFinish, listOfSlots, listOfExpectedSlots);
            return finaliseMethodTestSettings;
        }

        private static FinaliseMethodTestSettingsDTO getFinaliseTestSettingsBasedOnGivenListOfTimeSlots(int delay, int indexSlotToFinish, List<TimeSlotInSchedule> listOfSlots, List<TimeSlotInSchedule> listOfExpectedSlots)
        {
            var actualFinishTime = listOfSlots[indexSlotToFinish].FinishTime.AddMinutes(delay);

            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)listOfSlots[indexSlotToFinish].Id

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
        private static List<TimeSlotInSchedule> getSlotsListWith3Slots()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(listOfTasks[0], listOfTasks[1]);
            return listOfSlots;
        }

        private static List<TimeSlotInSchedule> getSlotsListWith5Slots()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            return listOfSlots;
        }

        private List<TimeSlotInSchedule> setupMockMethodsForFinaliseTimeSlot(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings,TimeOnly scheduleFinishTime, TimeOnly scheduleStartTime,int indexOfFinishedSlot)
        {
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = new List<TimeSlotInSchedule>();
            ScheduleSettings scheduleSettings = new ScheduleSettings()
            {
                FinishTime = scheduleFinishTime,
                Id = 1,
                StartTime = scheduleStartTime,
            };

            var listOfActiveSlots = new List<TimeSlotInSchedule>(finaliseMethodTestSettings.ListOfSlots);
            listOfActiveSlots.Remove(finaliseMethodTestSettings.ListOfSlots[indexOfFinishedSlot]);
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                .Callback((TimeSlotInSchedule slot) => listOfUpdatedTimeSlots.Add(slot));
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(finaliseMethodTestSettings.Model.SlotId))
                    .ReturnsAsync(finaliseMethodTestSettings.ListOfSlots[indexOfFinishedSlot]);
            _scheduleRespositorMock.Setup(m => m.GetScheduleSettings())
                .ReturnsAsync(scheduleSettings);
            _scheduleRespositorMock.Setup(m => m.GetActiveSlots())
                .ReturnsAsync(listOfActiveSlots);

            return listOfUpdatedTimeSlots;
        }


    }
}
