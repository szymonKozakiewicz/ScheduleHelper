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

namespace ScheduleHelper.ServiceTests
{
    public class TaskServiceTests
    {
        private TaskService _taskService;

        [Fact]
        public async Task AddNewTask_forValidData_ShouldCallAddNewTaskMethodFromRepository()
        {
            Mock<ITaskRespository>repositoryMock= new Mock<ITaskRespository>();
            repositoryMock.Setup(mock => mock.AddNewTask(It.IsAny<SingleTask>()));
            ITaskRespository taskRespository = repositoryMock.Object;
            _taskService=new TaskService(taskRespository);
            var model = new SingleTaskDTO()
            {
                Name = "Test",
                Time = 23
            };

            _taskService.AddNewTask(model);
            repositoryMock.Verify(mock => mock.AddNewTask(It.IsAny<SingleTask>()), Times.Once);


        }
    }
}
