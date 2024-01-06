using FluentAssertions;
using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static ScheduleHelper.Core.Services.Helpers.SingleTaskConvertHelper;

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
            
            repositoryMock.Setup(mock => mock.AddNewTask(It.IsAny<SingleTask>()));
            

            var model = new TaskCreateDTO()
            {
                Name = "Test",
                Time = 23
            };

            await _taskService.AddNewTask(model);


            //assert
            repositoryMock.Verify(mock => mock.AddNewTask(It.IsAny<SingleTask>()), Times.Once);


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

            var resultList= await _taskService.GetTasksList();


            resultList.Count.Should().Be(3);
            resultList.Should().Contain(covertSingleTaskToTaskForEditListDTO(task1));
            resultList.Should().Contain(covertSingleTaskToTaskForEditListDTO(task2));
            resultList.Should().Contain(covertSingleTaskToTaskForEditListDTO(task3));

        }

        [Fact]
        public async Task RemoveTaskWithId_ForValidId_RepositoryMehodRemoveTaskWithIdShouldBeCalled()
        {
            Guid id = new Guid("B6FA49D1-7FFA-44A7-8859-BE5FC94FBDF2");
            repositoryMock.Setup(mock => mock.RemoveTaskWithId(It.IsAny<Guid>()));


            await _taskService.RemoveTaskWithId(id);


            //assert
            repositoryMock.Verify(mock => mock.RemoveTaskWithId(It.IsAny<Guid>()), Times.Once);


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
            repositoryMock.Setup(mock => mock.GetTasks())
                .ReturnsAsync(tasksListsInMemory);

            var resultList = await _taskService.GetTasksForSchedule();


            resultList.Count.Should().Be(3);
            resultList.Should().Contain(covertSingleTaskToTaskForSheduleDTO(task1));
            resultList.Should().Contain(covertSingleTaskToTaskForSheduleDTO(task2));
            resultList.Should().Contain(covertSingleTaskToTaskForSheduleDTO(task3));

        }

    }
}
