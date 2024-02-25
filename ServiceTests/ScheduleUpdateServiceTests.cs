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
using Xunit;
using ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot;
using ScheduleHelper.ServiceTests.Helpers.Dto;
using ScheduleHelper.ServiceTests.ClassData.FinaliseTimeSlot.withFixedSlots;

namespace ScheduleHelper.ServiceTests
{
    public class ScheduleUpdateServiceTests:ScheduleUpdateServiceTestsBase
    {


        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forValidIdAndSlotFinishedOnTime))]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedOnTime_itShouldSetTimeSlotStatusForFinished(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {


            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
        }

        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forValidIdAndSlotFinishedOnTime))]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTime_itShouldUpdateAll(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule);


            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
        }



        [Theory]
        [ClassData(typeof(ArgumentsTestforValidIdAndSlotFinishedAfterTimeAndOtherSlotsShouldBeAfterMidnight))]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTimeAndAfterMidnight_itShouldUpdateAll(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
        }


        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forTimeSlotWhichIsNotFirstOnListSlotfinishedTooEarly))]
        public async Task FinaliseTimeSlot_forValidIdAndTimeSlotWhichIsNotFirstOnListslotfinishedTooEarly_itShouldUpdateSlotsAsExpected(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings,TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks,DaySchedule daySchedule)
        {
            
            

            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings,timeSlotsWithNoFinished,timeSlotsWithNoBreaks,daySchedule);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
            
        }


   

        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forTimeSlotWhichIsNotFirstOnListSlotfinishedBeforeScheduleStarted))]
        public async Task FinaliseTimeSlot_forValidIdAndTimeSlotWhichIsNotFirstOnListSlotfinishedBeforeScheduleStarted_itShouldUpdateSlotsAsExpected(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);

        }

        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_NotEnoughTimeForSomeSlots))]
        public async Task FinaliseTimeSlot_NotEnoughTimeForSomeSlots_itShouldHaveExpectedNumberOfCanceledSlots(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {

            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
            

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

        private static TimeSlotList getExpectedTimeSlotsForTest1(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings)
        {
            var expectedTimeSlots = finaliseMethodTestSettings.ListOfExpectedSlots;
            expectedTimeSlots.Remove(expectedTimeSlots[2]);
            
            expectedTimeSlots[0].StartTime = new TimeOnly(14, 4);
            expectedTimeSlots[0].FinishTime = new TimeOnly(15, 4);
            expectedTimeSlots[1].StartTime = new TimeOnly(15, 4);
            expectedTimeSlots[1].FinishTime = new TimeOnly(15, 24);
            expectedTimeSlots[2].StartTime = new TimeOnly(15, 24);
            expectedTimeSlots[2].FinishTime = new TimeOnly(15, 44);
            expectedTimeSlots[3].StartTime = new TimeOnly(15, 44);
            expectedTimeSlots[3].FinishTime = new TimeOnly(16, 10);
            return expectedTimeSlots;
        }

        private static TimeSlotList getExpectedTimeSlotsForTest2(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings)
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


        protected TimeSlotList setupMockMethodsForFinaliseTimeSlot(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings scheduleSettings,TimeSlotList slotsWithNoFinished,TimeSlotList slotsWithNotBreaks,DaySchedule daySchedule)
        {
            TimeSlotList listOfUpdatedTimeSlots = new TimeSlotList();
            TimeSlotList listOfAddedSlots = new TimeSlotList();


            var listOfActiveSlots = new TimeSlotList(finaliseMethodTestSettings.ListOfSlots);
            
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                .Callback((TimeSlotInSchedule slot) => listOfUpdatedTimeSlots.Add(slot));
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(finaliseMethodTestSettings.Model.SlotId))
                    .ReturnsAsync(finaliseMethodTestSettings.ListOfSlots[0]);
            _scheduleRespositorMock.Setup(m => m.GetScheduleSettings())
                .ReturnsAsync(scheduleSettings);
            _scheduleRespositorMock.SetupSequence(m => m.GetActiveSlots())
                .ReturnsAsync(slotsWithNoFinished)
                .ReturnsAsync(slotsWithNoFinished)
                .ReturnsAsync(slotsWithNotBreaks);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                    .Callback((TimeSlotInSchedule m) => listOfAddedSlots.Add(m));
            _scheduleRespositorMock.SetupSequence(m => m.GetDaySchedule())
                .ReturnsAsync(daySchedule.Copy())
                .ReturnsAsync(daySchedule.Copy());

            return listOfAddedSlots;
        }


        private  FinaliseMethodTestSettingsDTO getFinaliseMethodTestSettingsWith5TimeSlots(int delay, int indexSlotToFinish)
        {
            var listOfSlots = getSlotsListWith5Slots();
            var listOfExpectedSlots = getSlotsListWith5Slots();
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseTestSettingsBasedOnGivenListOfTimeSlots(delay, indexSlotToFinish, listOfSlots, listOfExpectedSlots);
            return finaliseMethodTestSettings;
        }






    }
}
