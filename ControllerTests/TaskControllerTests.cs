using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.ControllerTests
{
    public class TaskControllerTests
    {

        ITaskService _taskService;
        
        Mock<ITaskService> _taskServiceMock;


        [Fact]
        public async Task AddNewTask_IsModelCorrect()
        {
            var controller=new TasksController(_taskService);


            var result=(ViewResult)await controller.AddNewTask();

            result.ViewData.Model.Should().BeOfType<SingleTaskDTO>();
        }

        [Fact]
        public async Task AddNewTask_forValidTask_ShouldCallAddNewTaskMethodOfTaskService()
        {
            SetupMockForAddNewTaskMethodFromService();

            var controller = new TasksController(_taskService);
            var model = new SingleTaskDTO()
            {
                Name = "test",
                Time = 7
            };
            setMockForResponseStatus(controller);



            await controller.AddNewTask(model);



            _taskServiceMock.Verify(mock => mock.AddNewTask(It.IsAny<SingleTaskDTO>()), Times.Once);

        }

        private void SetupMockForAddNewTaskMethodFromService()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _taskService = _taskServiceMock.Object;
            _taskServiceMock.Setup(mock => mock.AddNewTask(It.IsAny<SingleTaskDTO>()));
        }

        private static void setMockForResponseStatus(TasksController controller)
        {
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupSet(r => r.StatusCode = (int)HttpStatusCode.Created);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(mock => mock.Response).Returns(httpResponseMock.Object);


            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object,
            };
            controller.ControllerContext = controllerContext;
        }
    }
}
