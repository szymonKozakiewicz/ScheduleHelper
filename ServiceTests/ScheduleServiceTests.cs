using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static ScheduleHelper.Core.Services.Helpers.SingleTaskConvertHelper;


namespace ScheduleHelper.ServiceTests
{
    public class ScheduleServiceTests
    {
        private IScheduleService _scheduleService;
        Mock<ITaskRespository> _taskRepositoryMock;
        Mock<IScheduleRepository> _scheduleRespositorMock;
        ITaskRespository _taskRespository;
        IScheduleRepository _scheduleRespository;

        public ScheduleServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRespository>();
            _scheduleRespositorMock=new Mock<IScheduleRepository>();
            _taskRespository = _taskRepositoryMock.Object;
            _scheduleRespository = _scheduleRespositorMock.Object;
            _scheduleService = new ScheduleService(_taskRespository,_scheduleRespository);
        }
        [Fact]
        public async Task GetTasksForSchedule_ForGivenTasksInMemory_ShouldReturnThisTasks()
        {

            var task1 = new SingleTask("test1", 15);
            SingleTask task2 = new SingleTask("test2", 14);
            SingleTask task3 = new SingleTask("test3", 12.3);

            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2,
                task3
            };
            _taskRepositoryMock.Setup(mock => mock.GetTasks())
                .ReturnsAsync(tasksListsInMemory);

            var resultList = await _scheduleService.GetTasksForSchedule();


            resultList.Count.Should().Be(3);
            resultList.Should().Contain(covertSingleTaskToTimeSlotInScheduleDTO(task1));
            resultList.Should().Contain(covertSingleTaskToTimeSlotInScheduleDTO(task2));
            resultList.Should().Contain(covertSingleTaskToTimeSlotInScheduleDTO(task3));

        }






        [Theory]
        [MemberData(nameof(GetArgumentsForGenerateScheduleTestWithNormalData))]
        public async Task GenerateSchedule_forNormalValidData_SchouldCallRepositoryAddMethods(ScheduleSettingsDTO testScheduleSettings, List<SingleTask> tasksListsInMemory, List<TimeSlotInSchedule> expectedTimeSlotsList)
        {

            var listOfSlotsPassedAsArgument = new List<TimeSlotInSchedule>();
            int expectedNumberOfTimeSlots = expectedTimeSlotsList.Count;

            _taskRepositoryMock.Setup(m => m.GetTasks()).ReturnsAsync(tasksListsInMemory);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>())).
                Callback((TimeSlotInSchedule timeSlot)=>listOfSlotsPassedAsArgument.Add(timeSlot));
            _scheduleRespositorMock.Setup(m => m.CleanTimeSlotInScheduleTable());




            //act
            await _scheduleService.GenerateSchedule(testScheduleSettings);


            //assert
            _scheduleRespositorMock.Verify(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()), Times.Exactly(expectedNumberOfTimeSlots));
            listOfSlotsPassedAsArgument.Should().BeEquivalentTo(expectedTimeSlotsList);
        }



        [Theory]
        [MemberData(nameof(GetArgumentsForGenerateScheduleTestWithValidDataButNotEnoughTimeForAllTasks))]
        public async Task GenerateSchedule_forValidDataButThereIsNotEnoughTimeForAllTasks_SchouldCallRepositoryAddMethods(ScheduleSettingsDTO testScheduleSettings, List<SingleTask> tasksListsInMemory, List<TimeSlotInSchedule> expectedTimeSlotsList)
        {

            var listOfSlotsPassedAsArgument = new List<TimeSlotInSchedule>();
            int expectedNumberOfTimeSlots = expectedTimeSlotsList.Count;

            _taskRepositoryMock.Setup(m => m.GetTasks()).ReturnsAsync(tasksListsInMemory);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>())).
                Callback((TimeSlotInSchedule timeSlot) => listOfSlotsPassedAsArgument.Add(timeSlot));
            _scheduleRespositorMock.Setup(m => m.CleanTimeSlotInScheduleTable());




            //act
            await _scheduleService.GenerateSchedule(testScheduleSettings);


            //assert
            _scheduleRespositorMock.Verify(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()), Times.Exactly(expectedNumberOfTimeSlots));
            listOfSlotsPassedAsArgument.Should().BeEquivalentTo(expectedTimeSlotsList);
        }



        #region arguments for tests
        public static IEnumerable<object[]> GetArgumentsForGenerateScheduleTestWithNormalData()
        {
            var testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 0),
            };


            var task1 = new SingleTask("test1", 60);
            SingleTask task2 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2
            };
            var expectedStartTime1 = new TimeOnly(12, 24);
            var expectedFinishTime1 = new TimeOnly(13, 24);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();
            var expectedStartTime2 = new TimeOnly(13, 24);
            var expectedFinishTime2 = new TimeOnly(13, 44);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime2)
                .SetFinishTime(expectedFinishTime2)
                .SetIsItBreak(true)
                .SetOrdinalNumber(2)
                .SetTask(null)
                .Build();
            var expectedStartTime3 = new TimeOnly(13, 44);
            var expectedFinishTime3 = new TimeOnly(14, 14);
            var expectedTimeSlot3= new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime3)
                .SetFinishTime(expectedFinishTime3)
                .SetIsItBreak(false)
                .SetOrdinalNumber(3)
                .SetTask(task2)
                .Build();
            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2,expectedTimeSlot3
            };
            yield return new object[] { testScheduleSettings,tasksListsInMemory, expectedTimeSlotsList};

           
        }


        public static IEnumerable<object[]> GetArgumentsForGenerateScheduleTestWithValidDataButNotEnoughTimeForAllTasks()
        {


            var testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(14, 0),
            };
            var task1 = new SingleTask("test1", 60);
            var task2 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2
            };
            var expectedStartTime1 = new TimeOnly(12, 24);
            var expectedFinishTime1 = new TimeOnly(13, 24);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };
            
            
            testScheduleSettings = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(13, 30),
            };
            task1 = new SingleTask("test1", 60);
            task2 = new SingleTask("test2", 30);
            tasksListsInMemory = new()
            {
                task1,
                task2
            };
            expectedStartTime1 = new TimeOnly(12, 24);
            expectedFinishTime1 = new TimeOnly(13, 24);
            expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();

            expectedTimeSlotsList = new()
            {
                expectedTimeSlot1
            };
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };
        }
        #endregion
    }
}
