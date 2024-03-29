﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ScheduleHelper.ControllerTests.Helpers;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Core.ServiceContracts;
using ScheduleHelper.Core.Services;
using ScheduleHelper.Infrastructure.Repositories;
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
        IScheduleService _scheduleService;
        Mock<ITaskService> _taskServiceMock;
        Mock<IScheduleService> _scheduleServiceMock;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _scheduleServiceMock=new Mock<IScheduleService>();
            _scheduleService = _scheduleServiceMock.Object;
            _taskService = _taskServiceMock.Object;
        }

        [Fact]
        public async Task AddNewTask_IsModelCorrect()
        {
            
            var controller=new TasksController(_taskService, _scheduleService);

            //act
            var result=(ViewResult)await controller.AddNewTask();

            //assert
            result.ViewData.Model.Should().BeOfType<TaskCreateDTO>();
        }

        [Fact]
        public async Task AddNewTask_forValidTask_ShouldCallAddNewTaskMethodOfTaskService()
        {
            SetupMockForAddNewTaskMethodFromService();

            var controller = new TasksController(_taskService, _scheduleService);
            var model = new TaskCreateDTO()
            {
                Name = "test",
                Time = 7
            };
            ServiceTestHelpers.setMockForResponseStatus(controller, HttpStatusCode.Created);


            //act
            await controller.AddNewTask(model);


            //assert
            _taskServiceMock.Verify(mock => mock.AddNewTask(It.IsAny<TaskCreateDTO>()), Times.Once);

        }



        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-0.5)]
        public async Task AddNewTask_forInvalidTask_ShouldNotCallAddNewTaskMethodOfTaskService(double time)
        {
            SetupMockForAddNewTaskMethodFromService();

            var controller = new TasksController(_taskService, _scheduleService);
            var model = new TaskCreateDTO()
            {
                Name = "test",
                Time = time
            };
            ServiceTestHelpers.setMockForResponseStatus(controller,HttpStatusCode.Created);


            //act
            await controller.AddNewTask(model);


            //assert
            _taskServiceMock.Verify(mock => mock.AddNewTask(It.IsAny<TaskCreateDTO>()), Times.Never);

        }


        [Fact]
        public async Task DeleteTask_ForValidId_ShouldCallMethodRemoveTaskWithId()
        {
            _taskServiceMock.Setup(mock => mock.RemoveTaskWithId(It.IsAny<Guid>()));
            
            var idToDelete = new Guid("05EB870B-B102-4E5F-B04F-17671345E956");
            var controller = new TasksController(_taskService, _scheduleService);
            ServiceTestHelpers.setMockForResponseStatus(controller, HttpStatusCode.OK);

            //act
            await controller.DeleteTask(idToDelete);

            //assert
            _taskServiceMock.Verify(mock => mock.RemoveTaskWithId(It.IsAny<Guid>()), Times.Once);
        }

 
        private void SetupMockForAddNewTaskMethodFromService()
        {

            _taskServiceMock.Setup(mock => mock.AddNewTask(It.IsAny<TaskCreateDTO>()));
        }


    }
}
