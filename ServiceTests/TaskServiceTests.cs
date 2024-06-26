﻿using FluentAssertions;
using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.Services;
using ScheduleHelper.Core.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static ScheduleHelper.Core.Services.Helpers.DtoToEnityConverter;

namespace ScheduleHelper.ServiceTests
{
    public class TaskServiceTests
    {
        private TaskService _taskService;
        Mock<ITaskRespository> repositoryMock;
        ITaskRespository taskRespository;

        public TaskServiceTests()
        {
            repositoryMock = new Mock<ITaskRespository>();
            taskRespository = repositoryMock.Object;
            _taskService = new TaskService(taskRespository);
        }
        [Fact]
        public async Task AddNewTask_forValidData_ShouldCallAddNewTaskMethodFromRepository()
        {
            SingleTask? result=null;
            repositoryMock.Setup(mock => mock.AddNewTask(It.IsAny<SingleTask>()))
                .Callback((SingleTask task) => result = task);
            

            var model = new TaskCreateDTO()
            {
                Name = "Test",
                Time = 23,
                HasStartTime = true,
                StartTime = new TimeOnly(8, 0)

            };

            SingleTask expectedResult = new SingleTask()
            {
                Name = "Test",
                HasStartTime = true,
                TimeMin = 23,
                StartTime = new TimeOnly(8, 0)
                
            };

            await _taskService.AddNewTask(model);


            //assert
            
            result.Should().Be(expectedResult);

        }

        [Fact]
        public async Task GetTasksList_ForGivenTasksInMemory_ShouldReturnThisTasks()
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
            repositoryMock.Setup(mock => mock.GetTasks())
                .ReturnsAsync(tasksListsInMemory);

            //act
            var resultList= await _taskService.GetTasksList();

            //assert
            resultList.Count.Should().Be(3);
            resultList.Should().Contain(EntityToDtoConverter.ConvertSingleTaskToTaskCreatDTO((task1)));
            resultList.Should().Contain(EntityToDtoConverter.ConvertSingleTaskToTaskCreatDTO((task2)));
            resultList.Should().Contain(EntityToDtoConverter.ConvertSingleTaskToTaskCreatDTO((task3)));

        }

        [Fact]
        public async Task RemoveTaskWithId_ForValidId_RepositoryMehodRemoveTaskWithIdShouldBeCalled()
        {
            Guid id = new Guid("B6FA49D1-7FFA-44A7-8859-BE5FC94FBDF2");
            repositoryMock.Setup(mock => mock.RemoveTaskWithId(It.IsAny<Guid>()));

            //act
            await _taskService.RemoveTaskWithId(id);


            //assert
            repositoryMock.Verify(mock => mock.RemoveTaskWithId(It.IsAny<Guid>()), Times.Once);


        }


        [Fact]
        public async Task UpdateTask_ForValidId_RepositoryMehodUpdateTaskShouldBeCalled()
        {
            SingleTask? result = null;


            var model = new TaskCreateDTO()
            {
                Name = "Test",
                Time = 23,
                HasStartTime = true,
                StartTime = new TimeOnly(8, 0),
                Id= Guid.NewGuid()

            };

            SingleTask expectedResult = new SingleTask()
            {
                Name = "Test",
                HasStartTime = true,
                TimeMin = 23,
                StartTime = new TimeOnly(8, 0),
                Id = model.Id

            };
            repositoryMock.Setup(mock => mock.UpdateTask(It.IsAny<SingleTask>()))
                .Callback((SingleTask task) => result = task);
            repositoryMock.Setup(mock => mock.GetTask(It.IsAny<Guid>()))
                .ReturnsAsync(expectedResult);
            
            //act
            await _taskService.UpdateTask(model);


            //assert

            result.Should().Be(expectedResult);
            repositoryMock.Verify(a=>a.UpdateTask(It.IsAny<SingleTask>()),Times.Once);


        }
        [Fact]
        public async Task GetTaskCreateDTOWithId_forValidId_ExpectThatItWillReturnExpectedTask()
        {
            //arrange
            SingleTask taskToReturn = new SingleTask()
            {
                Id = Guid.NewGuid(),
                HasStartTime = true,
                Name = "testTask",
                StartTime = new TimeOnly(1, 2),
                TimeMin = 20
            };
            TaskCreateDTO expectedTaskCreateDTO = new TaskCreateDTO()
            {
                Id = taskToReturn.Id,
                HasStartTime = true,
                Name = "testTask",
                StartTime = new TimeOnly(1, 2),
                Time = 20
            };
            SingleTask secoundTask = new SingleTask()
            {
                Id = Guid.NewGuid(),
                HasStartTime = true,
                Name = "testTask2",
                StartTime = new TimeOnly(1, 50),
                TimeMin = 30
            };
            List<SingleTask> tasks = new List<SingleTask>()
            {
                secoundTask,taskToReturn
            };
            repositoryMock.Setup(a=>a.GetTasks())
                .ReturnsAsync(tasks);

            //act
            TaskCreateDTO result=await _taskService.GetTaskCreateDTOWithId(taskToReturn.Id);

            //assert
            result.Should().Be(expectedTaskCreateDTO);
            result.Id.Should().Be(taskToReturn.Id);
            
        }


    }
}
