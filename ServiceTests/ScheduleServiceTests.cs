using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using ScheduleHelper.ServiceTests.ClassData;
using ScheduleHelper.ServiceTests.Helpers;
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
        [ClassData(typeof(GenerateScheduleTestWithNormalData))]
        public async Task GenerateSchedule_forNormalValidData_SchouldCallRepositoryAddMethods(ScheduleSettingsDTO testScheduleSettings, List<SingleTask> tasksListsInMemory, List<TimeSlotInSchedule> expectedTimeSlotsList)
        {

            var listOfSlotsPassedAsArgument = new List<TimeSlotInSchedule>();
            int expectedNumberOfTimeSlots = expectedTimeSlotsList.Count;

            setupMockMethods(tasksListsInMemory, listOfSlotsPassedAsArgument);




            //act
            await _scheduleService.GenerateSchedule(testScheduleSettings);


            //assert
            _scheduleRespositorMock.Verify(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()), Times.Exactly(expectedNumberOfTimeSlots));
            listOfSlotsPassedAsArgument.Should().BeEquivalentTo(expectedTimeSlotsList);
        }



        [Theory]
        [ClassData(typeof(ArgumentsForGenerateScheduleTestWithValidDataButNotEnoughTimeForAllTasks))]
        public async Task GenerateSchedule_forValidDataButThereIsNotEnoughTimeForAllTasks_SchouldCallRepositoryAddMethods(ScheduleSettingsDTO testScheduleSettings, List<SingleTask> tasksListsInMemory, List<TimeSlotInSchedule> expectedTimeSlotsList)
        {

            var listOfSlotsPassedAsArgument = new List<TimeSlotInSchedule>();
            int expectedNumberOfTimeSlots = expectedTimeSlotsList.Count;

            setupMockMethods(tasksListsInMemory, listOfSlotsPassedAsArgument);




            //act
            await _scheduleService.GenerateSchedule(testScheduleSettings);


            //assert
            _scheduleRespositorMock.Verify(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>()), Times.Exactly(expectedNumberOfTimeSlots));
            listOfSlotsPassedAsArgument.Should().BeEquivalentTo(expectedTimeSlotsList);
        }


        [Theory]
        [ClassData(typeof(ArgumentsForGenerateScheduleForValidDataButOneTaskHaveToBeSplitted))]
        public async Task GenerateSchedule_forValidDataButOneTaskHaveToBeSplitted_SchouldCallRepositoryAddMethods(ScheduleSettingsDTO testScheduleSettings, List<SingleTask> tasksListsInMemory, List<TimeSlotInSchedule> expectedTimeSlotsList)
        {

            var listOfSlotsPassedAsArgument = new List<TimeSlotInSchedule>();
            int expectedNumberOfTimeSlots = expectedTimeSlotsList.Count;

            setupMockMethods(tasksListsInMemory, listOfSlotsPassedAsArgument);




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
            var tasksList = GenerateDataHelper.GetNormalValidListOfTasks();
            List<TimeSlotInSchedule> timeSlotsList = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(tasksList[0], tasksList[1]);
            MockSetupHelper.SetupMockForGetTimeSlotsListMethodForGivenTimeSlotList(timeSlotsList,_scheduleRespositorMock);
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
            var tasksList = GenerateDataHelper.GetNormalValidListOfTasks();
            List<TimeSlotInSchedule> timeSlotsList = GenerateDataHelper.GetTimeSlotsListWith3TimeSlots(tasksList[0], tasksList[1]);
            var notSortedTimeSlotsList = new List<TimeSlotInSchedule>()
            {
                timeSlotsList[1],timeSlotsList[0],timeSlotsList[2]
            };
            MockSetupHelper.SetupMockForGetTimeSlotsListMethodForGivenTimeSlotList(notSortedTimeSlotsList,_scheduleRespositorMock);

            //act

            var resultList= await _scheduleService.GetTimeSlotsList(); 


            //assert

            resultList[0].OrdinalNumber.Should().Be(1);
            resultList[1].OrdinalNumber.Should().Be(2);
            resultList[2].OrdinalNumber.Should().Be(3);

        }

        [Fact]
        public async Task GetTasksNotSetInSchedule_ShouldCallGetTasksNotSetInScheduleMethodFromRepository()
        {
            _scheduleRespositorMock.Setup(m => m.GetTasksNotSetInSchedule())
                .ReturnsAsync(new List<SingleTask>());

            //act
            await _scheduleService.GetTasksNotSetInSchedule();


            //assert
            _scheduleRespositorMock.Verify(m => m.GetTasksNotSetInSchedule(), Times.Once);


        }


        [Fact]
        public async Task GetShareOfSlotsWithStatus_forValidData_ShouldReturnExpectedResults()
        {

            List<TimeSlotInSchedule> slots = new List<TimeSlotInSchedule>()
            {
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0), Status=TimeSlotStatus.Finished},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Active},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Active},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Active},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Canceled},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Finished},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Active},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(3,0),Status=TimeSlotStatus.Canceled},
                new TimeSlotInSchedule(){StartTime=new TimeOnly(1,0),FinishTime=new TimeOnly(2,0),Status=TimeSlotStatus.Active}
                
            };
            _scheduleRespositorMock.Setup(m => m.GetTimeSlotsList())
                .ReturnsAsync(slots);

            //act
            var active=await _scheduleService.GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus.Active);
            var finished=await _scheduleService.GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus.Finished);
            var canceled=await _scheduleService.GetShareOfTimeOfSlotsWithStatus(TimeSlotStatus.Canceled);


            //assert
            active.Should().Be(50);
            finished.Should().Be(20);
            canceled.Should().Be(30);


        }


        private void setupMockMethods(List<SingleTask> tasksListsInMemory, List<TimeSlotInSchedule> listOfSlotsPassedAsArgument)
        {
            _taskRepositoryMock.Setup(m => m.GetTasks()).ReturnsAsync(tasksListsInMemory);
            _scheduleRespositorMock.Setup(m => m.AddNewTimeSlot(It.IsAny<TimeSlotInSchedule>())).
                Callback((TimeSlotInSchedule timeSlot) => listOfSlotsPassedAsArgument.Add(timeSlot));
            _scheduleRespositorMock.Setup(m => m.CleanTimeSlotInScheduleTable());
            _scheduleRespositorMock.Setup(m => m.UpdateScheduleSettings(It.IsAny<ScheduleSettings>()));
        }
    }
}
