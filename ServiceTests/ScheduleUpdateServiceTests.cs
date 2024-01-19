using FluentAssertions;
using Moq;
using ScheduleHelper.Core.Domain.Entities;
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
            var listOfTasks = GenerateDataHelper.GetNormalValidListOfTasks();
            var listOfSlots = GenerateDataHelper.GetTimeSlotsList(listOfTasks[0], listOfTasks[1]);
            var listOfExpectedSlots = GenerateDataHelper.GetTimeSlotsList(listOfTasks[0], listOfTasks[1]);
            var actualFinishTime = listOfSlots[0].FinishTime.AddMinutes(delay);
            ScheduleSettings scheduleSettings = new ScheduleSettings()
            {
                FinishTime = new TimeOnly(20, 0),
                Id = 1
            };
            List<TimeSlotInSchedule>listOfUpdatedTimeSlots=new List<TimeSlotInSchedule>();
            FinaliseSlotDTO model = new FinaliseSlotDTO()
            {
                FinishTime = actualFinishTime.ToString(),
                SlotId = (Guid)listOfSlots[0].Id

            };
            var listOfActiveSlots = new List<TimeSlotInSchedule>(listOfSlots);
            listOfActiveSlots.Remove(listOfSlots[0]);


            _scheduleRespositorMock.Setup(m => m.UpdateTimeSlot(It.IsAny<TimeSlotInSchedule>()))
                .Callback((TimeSlotInSchedule slot)=>listOfUpdatedTimeSlots.Add(slot));
            _scheduleRespositorMock.Setup(m => m.GetTimeSlot(model.SlotId))
                    .ReturnsAsync(listOfSlots[0]);
            _scheduleRespositorMock.Setup(m => m.GetScheduleSettings())
                .ReturnsAsync(scheduleSettings);
            _scheduleRespositorMock.Setup(m => m.GetActiveSlots())
                .ReturnsAsync(listOfActiveSlots);

            //act
            await _scheduleUpdateService.FinaliseTimeSlot(model);

            //assert
            listOfUpdatedTimeSlots.Should().HaveCount(3);
            foreach(var updatedSlot in listOfUpdatedTimeSlots)
            {
                bool slotWasOnExpectedList = false;
                if (updatedSlot.Id == listOfSlots[0].Id)
                    continue;
                foreach (var expectedTimeSlot in listOfExpectedSlots)
                {
                    TimeOnly expectedFinishTime = expectedTimeSlot.FinishTime.AddMinutes(delay);
                    TimeOnly expectedStartTime = expectedTimeSlot.StartTime.AddMinutes(delay);
                    bool startTimeSame = expectedStartTime.AreTimesEqualWithTolerance(updatedSlot.StartTime);
                    bool finishTimeSame = expectedFinishTime.AreTimesEqualWithTolerance(updatedSlot.FinishTime);
                    if (startTimeSame && finishTimeSame)
                    {
                        slotWasOnExpectedList = true;
                        break;
                    }
                }  
                slotWasOnExpectedList.Should().BeTrue();    
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
    }
}
