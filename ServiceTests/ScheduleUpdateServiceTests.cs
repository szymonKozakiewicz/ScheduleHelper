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
using ScheduleHelper.ServiceTests.ClassData;
using ScheduleHelper.Core.Services.Helpers;

namespace ScheduleHelper.ServiceTests
{
    public class ScheduleUpdateServiceTests:ScheduleUpdateServiceTestsBase
    {


        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forValidIdAndSlotFinishedOnTime))]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedOnTime_itShouldSetTimeSlotStatusForFinished(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {


            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());

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
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());


            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
        }


        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forDayScheduleWhichHasNotZeroWorkTime))]
        public async Task FinaliseTimeSlot_forDayScheduleWhichHasNotZeroWorkTime(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule,double expectedDayInScheduleTimeOfWork)
        {
            var dayInSchedule1 = new DaySchedule()
            {
                TimeFromLastBreakMin = expectedDayInScheduleTimeOfWork-30
            };
            var dayInSchedule2 = new DaySchedule()
            {
                TimeFromLastBreakMin = expectedDayInScheduleTimeOfWork
            };
            var updatedDayInSchedule = new DaySchedule();
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());
            _scheduleRespositorMock.SetupSequence(m => m.GetDaySchedule())
                .ReturnsAsync(dayInSchedule1)
                .ReturnsAsync(dayInSchedule2);
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(finaliseMethodTestSettings.Model.SlotId))
                    .ReturnsAsync(finaliseMethodTestSettings.ListOfSlots[2]);
            _scheduleRespositorMock.SetupSequence(m => m.GetDaySchedule())
                    .ReturnsAsync(dayInSchedule1)
                    .ReturnsAsync(dayInSchedule2);
            _scheduleRespositorMock.Setup(m => m.UpdateDaySchedule(It.IsAny<DaySchedule>()))
                .Callback((DaySchedule dayInSchedule) => updatedDayInSchedule = dayInSchedule);


            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfUpdatedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
            updatedDayInSchedule.TimeFromLastBreakMin.Should().BeApproximately(expectedDayInScheduleTimeOfWork,0.1);
        }


        [Theory]
        [ClassData(typeof(ArgumentsTestforValidIdAndSlotFinishedAfterTimeAndOtherSlotsShouldBeAfterMidnight))]
        public async Task FinaliseTimeSlot_forValidIdAndSlotFinishedAfterTimeAndAfterMidnight_itShouldUpdateAll(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());

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
            
            

            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings,timeSlotsWithNoFinished,timeSlotsWithNoBreaks,daySchedule, new List<TimeSlotInSchedule>());

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
            TimeSlotList listOfUpdatedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());

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

            TimeSlotList listOfAddedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert

            listOfAddedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfAddedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);
            

        }


        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_TwoCanceledSlotsWhichShouldBeJoined))]
        public async Task FinaliseTimeSlot_TwoCanceledSlotsWhichShouldBeJoined(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule,TimeSlotList listOfCanceledSlots)
        {
            TimeSlotList updatedSlots = new TimeSlotList();
            TimeSlotList deletedSlots = new TimeSlotList();
            setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, listOfCanceledSlots);
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                    .Callback((TimeSlotInSchedule m) => updatedSlots.Add(m));
            _scheduleRespositorMock.Setup(m => m.RemoveTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                    .Callback((TimeSlotInSchedule m) => deletedSlots.Add(m));
            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert
            var listOfCancedeSlots= updatedSlots.FindAll(slot => slot.Status == TimeSlotStatus.Canceled).ToList();
            deletedSlots= deletedSlots.FindAll(slot => slot.Status == TimeSlotStatus.Canceled).ToList();
            listOfCancedeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);
            deletedSlots.Should().HaveCount(1);
            listOfCancedeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);


        }

        [Theory]
        [ClassData(typeof(ArgumentsFinaliseTimeSlot_forValidIdAndTimeSlotSlotFinishedAfterScheduleFinished))]
        public async Task FinaliseTimeSlot_forValidIdAndTimeSlotSlotFinishedAfterScheduleFinished(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings settings, TimeSlotList timeSlotsWithNoFinished, TimeSlotList timeSlotsWithNoBreaks, DaySchedule daySchedule)
        {


            TimeSlotList listOfAddedTimeSlots = setupMockMethodsForFinaliseTimeSlot(finaliseMethodTestSettings, settings, timeSlotsWithNoFinished, timeSlotsWithNoBreaks, daySchedule, new List<TimeSlotInSchedule>());

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(finaliseMethodTestSettings.Model);

            //assert

            listOfAddedTimeSlots.Should().HaveCount(finaliseMethodTestSettings.ListOfExpectedSlots.Count);

            listOfAddedTimeSlots.Should().BeEquivalentTo(finaliseMethodTestSettings.ListOfExpectedSlots);


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



        [Theory]
        [ClassData(typeof(ArgumentsUpdateSettings_ForValidSettings_itShouldUpdateSettingsAndUpdateSchedule))]
        public async Task UpdateSettings_ForValidSettings_itShouldUpdateSettingsAndUpdateSchedule(TimeSlotList initTimeSlots,TimeSlotList expectedTimeSlots, ScheduleSettings settings, DaySchedule daySchedule)
        {

            TimeSlotList listOfAddedSlots = new TimeSlotList();
            ScheduleSettings? updatedSettings = null;
            TimeSlotList slotsWithNoBreaks = new TimeSlotList(initTimeSlots);
            slotsWithNoBreaks.RemoveAt(1);
            slotsWithNoBreaks.RemoveAt(2);
            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()));
            _scheduleRespositorMock.Setup(m => m.GetScheduleSettings())
                .ReturnsAsync(settings);
            _scheduleRespositorMock.SetupSequence(m => m.GetActiveSlots())
                .ReturnsAsync(initTimeSlots)
                .ReturnsAsync(initTimeSlots)
                .ReturnsAsync(initTimeSlots)
                .ReturnsAsync(slotsWithNoBreaks);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                    .Callback((TimeSlotInSchedule m) => listOfAddedSlots.Add(m));
            _scheduleRespositorMock.SetupSequence(m => m.GetDaySchedule())
                .ReturnsAsync(daySchedule.Copy())
                .ReturnsAsync(daySchedule.Copy());
            _scheduleRespositorMock.Setup(m => m.GetCanceledSlots())
                    .ReturnsAsync(new TimeSlotList());
            _scheduleRespositorMock.Setup(m => m.UpdateScheduleSettings(It.IsAny<ScheduleSettings>()))
                .Callback((ScheduleSettings n)=>updatedSettings=n);

            var settingsDto=DtoToEnityConverter.ConvertScheduleSettingsToDto(settings);
            await _scheduleUpdateService.UpdateSettings(settingsDto);


            //assert
            listOfAddedSlots.Should().BeEquivalentTo(expectedTimeSlots);
            updatedSettings.Should().NotBeNull();

        }





        protected TimeSlotList setupMockMethodsForFinaliseTimeSlot(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, ScheduleSettings scheduleSettings,TimeSlotList slotsWithNoFinished,TimeSlotList slotsWithNotBreaks,DaySchedule daySchedule,TimeSlotList listOfCanceledSlots)
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
            _scheduleRespositorMock.Setup(m => m.GetCanceledSlots())
                    .ReturnsAsync(listOfCanceledSlots);
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
