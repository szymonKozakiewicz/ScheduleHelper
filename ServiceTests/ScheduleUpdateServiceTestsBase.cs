using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using ScheduleHelper.ServiceTests.Helpers;
using ScheduleHelper.ServiceTests.Helpers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests
{
    public class ScheduleUpdateServiceTestsBase
    {
        protected IScheduleUpdateService _scheduleUpdateService;
        protected Mock<ITaskRespository> _taskRepositoryMock;
        protected Mock<IScheduleRepository> _scheduleRespositorMock;
        protected ITaskRespository _taskRespository;
        protected IScheduleRepository _scheduleRespository;

        public ScheduleUpdateServiceTestsBase()
        {
            _taskRepositoryMock = new Mock<ITaskRespository>();
            _scheduleRespositorMock = new Mock<IScheduleRepository>();
            _taskRespository = _taskRepositoryMock.Object;
            _scheduleRespository = _scheduleRespositorMock.Object;
            _scheduleUpdateService = new ScheduleUpdateService(_scheduleRespository);
        }


        protected FinaliseMethodTestSettingsDTO getFinaliseMethodTestSettings(int delay, int indexSlotToFinish)
        {
            var listOfSlots = getSlotsListWith3Slots();
            var listOfExpectedSlots = getSlotsListWith3Slots();
            FinaliseMethodTestSettingsDTO finaliseMethodTestSettings = getFinaliseTestSettingsBasedOnGivenListOfTimeSlots(delay, indexSlotToFinish, listOfSlots, listOfExpectedSlots);
            return finaliseMethodTestSettings;
        }

        protected FinaliseMethodTestSettingsDTO getFinaliseTestSettingsBasedOnGivenListOfTimeSlots(int delay, int indexSlotToFinish, List<TimeSlotInSchedule> listOfSlots, List<TimeSlotInSchedule> listOfExpectedSlots)
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

        protected static List<TimeSlotInSchedule> getSlotsListWith3Slots()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(listOfTasks[0], listOfTasks[1]);
            return listOfSlots;
        }

        protected static List<TimeSlotInSchedule> getSlotsListWith5Slots()
        {
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsListWith5TimeSlots(listOfTasks[0], listOfTasks[1]);
            return listOfSlots;
        }


        protected List<TimeSlotInSchedule> setupMockMethodsForFinaliseTimeSlot(FinaliseMethodTestSettingsDTO finaliseMethodTestSettings, TimeOnly scheduleFinishTime, TimeOnly scheduleStartTime, int indexOfFinishedSlot)
        {
            
            List<TimeSlotInSchedule> listOfUpdatedTimeSlots = new List<TimeSlotInSchedule>();
            ScheduleSettings scheduleSettings = new ScheduleSettings()
            {
                FinishTime = scheduleFinishTime,
                Id = 1,
                StartTime = scheduleStartTime,
            };
            var slotTofinish = finaliseMethodTestSettings.ListOfSlots[indexOfFinishedSlot];
            var listOfActiveSlots = new List<TimeSlotInSchedule>(finaliseMethodTestSettings.ListOfSlots);
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
