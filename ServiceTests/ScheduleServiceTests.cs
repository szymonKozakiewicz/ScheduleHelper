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

        [Fact]
        public async Task GetTimeSlotsList_ForValidData_ReturnsTimeSlotList()
        {
            //arrange
            var tasksList = getNormalValidListOfTasks();
            List<TimeSlotInSchedule> timeSlotsList = GetTimeSlotsList(tasksList[0], tasksList[1]);
            setupMockForGetTimeSlotsListMethodForGivenTimeSlotList(timeSlotsList);
            List<TimeSlotInScheduleDTO> expectedResultList = new List<TimeSlotInScheduleDTO>()
            {
                new TimeSlotInScheduleDTO()
                {
                    Id=new Guid("1FA2A22C-4ADC-4123-8D12-8300973DC046"),
                    StartTime=new TimeOnly(12, 24),
                    FinishTime=new TimeOnly(13, 24),
                    OrdinalNumber=1,
                    Name=tasksList[0].Name
                },
                new TimeSlotInScheduleDTO()
                {
                    Id=new Guid("1FA2A22C-4ADC-4123-8D12-8300973DC046"),
                    StartTime=new TimeOnly(13, 24),
                    FinishTime=new TimeOnly(13, 44),
                    OrdinalNumber=2,
                    Name="Break"
                },
                new TimeSlotInScheduleDTO()
                {
                    Id=new Guid("1FA2A22C-4ADC-4123-8D12-8300973DC046"),
                    StartTime=new TimeOnly(13, 44),
                    FinishTime=new TimeOnly(14, 14),
                    OrdinalNumber=3,
                    Name=tasksList[1].Name
                }
            };


            //act
            var resultList = await _scheduleService.GetTimeSlotsList();


            //assert
            resultList.Should().BeEquivalentTo(expectedResultList);



        }



        [Fact]
        public async Task GetTimeSlotsList_ForDataWhichArenTSorted_ReturnsSortedTimeSlotList()
        {
            //arrange
            var tasksList = getNormalValidListOfTasks();
            List<TimeSlotInSchedule> timeSlotsList = GetTimeSlotsList(tasksList[0], tasksList[1]);
            var notSortedTimeSlotsList = new List<TimeSlotInSchedule>()
            {
                timeSlotsList[1],timeSlotsList[0],timeSlotsList[2]
            };
            setupMockForGetTimeSlotsListMethodForGivenTimeSlotList(notSortedTimeSlotsList);

            //act

            var resultList= await _scheduleService.GetTimeSlotsList();


            //assert

            resultList[0].OrdinalNumber.Should().Be(1);
            resultList[1].OrdinalNumber.Should().Be(2);
            resultList[2].OrdinalNumber.Should().Be(3);

        }

        #region helpMethods
        private void setupMockForGetTimeSlotsListMethodForGivenTimeSlotList(List<TimeSlotInSchedule> timeSlotsList)
        {
            _scheduleRespositorMock.Setup(m => m.GetTimeSlotsList()).ReturnsAsync(timeSlotsList);
        }
        #endregion

        #region arguments for tests
        public static IEnumerable<object[]> GetArgumentsForGenerateScheduleTestWithNormalData()
        {
            ScheduleSettingsDTO testScheduleSettings = getNormalValidScheduleSettings();

            List<SingleTask> tasksListsInMemory = getNormalValidListOfTasks();
            List<TimeSlotInSchedule> expectedTimeSlotsList = GetTimeSlotsList(tasksListsInMemory[0], tasksListsInMemory[1]);
            yield return new object[] { testScheduleSettings, tasksListsInMemory, expectedTimeSlotsList };


        }

        private static List<SingleTask> getNormalValidListOfTasks()
        {
            var task1 = new SingleTask("test1", 60);
            SingleTask task2 = new SingleTask("test2", 30);
            List<SingleTask> tasksListsInMemory = new()
            {
                task1,
                task2
            };
            return tasksListsInMemory;
        }

        private static ScheduleSettingsDTO getNormalValidScheduleSettings()
        {
            return new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 0),
            };
        }

        private static List<TimeSlotInSchedule> GetTimeSlotsList(SingleTask task1, SingleTask task2)
        {
            var expectedStartTime1 = new TimeOnly(12, 24);
            var expectedFinishTime1 = new TimeOnly(13, 24);
            var expectedTimeSlot1 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime1)
                .SetFinishTime(expectedFinishTime1)
                .SetIsItBreak(false)
                .SetId(new Guid("1FA2A22C-4ADC-4123-8D12-8300973DC046"))
                .SetOrdinalNumber(1)
                .SetTask(task1)
                .Build();
            var expectedStartTime2 = new TimeOnly(13, 24);
            var expectedFinishTime2 = new TimeOnly(13, 44);
            var expectedTimeSlot2 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime2)
                .SetFinishTime(expectedFinishTime2)
                .SetId(new Guid("1FA2A22C-4ADC-4123-8D12-8300973DC046"))
                .SetIsItBreak(true)
                .SetOrdinalNumber(2)
                .SetTask(null)
                .Build();
            var expectedStartTime3 = new TimeOnly(13, 44);
            var expectedFinishTime3 = new TimeOnly(14, 14);
            var expectedTimeSlot3 = new TimeSlotInScheduleBuilder()
                .SetStartTime(expectedStartTime3)
                .SetFinishTime(expectedFinishTime3)
                .SetId(new Guid("1FA2A22C-4ADC-4123-8D12-8300973DC046"))
                .SetIsItBreak(false)
                .SetOrdinalNumber(3)
                .SetTask(task2)
                .Build();
            List<TimeSlotInSchedule> expectedTimeSlotsList = new()
            {
                expectedTimeSlot1,expectedTimeSlot2,expectedTimeSlot3
            };
            return expectedTimeSlotsList;
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
