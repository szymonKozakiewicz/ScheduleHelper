﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Infrastructure;
using ScheduleHelper.IntegrationTests.ClassData;
using ScheduleHelper.IntegrationTests.Helpers;
using ScheduleHelper.UI;
using ScheduleHelper.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.IntegrationTests
{
    public class TasksControllerIntegrationTests:IClassFixture<InMemoryWebApplicationFactory>
    {
        HttpClient _client;
        MyDbContext _dbContext;

        public TasksControllerIntegrationTests(InMemoryWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            _dbContext = factory.GetDbContextInstance();

        }

        [Fact]
        public async Task GetTasksListView_ShouldReturnView()
        {

            //act
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.ShowTasksList);
            
            //assert
            response.Should().BeSuccessful();

            
        }
        [Fact]
        public async Task UpdateTaskGet_ShouldBeSuccessful()
        {

            var testTask = new SingleTask("Test", 234);

            _dbContext.Add(testTask);
            _dbContext.SaveChanges();
            string route = RouteConstants.UpdateTask + "?taskToEditId=" + testTask.Id.ToString();
            //act
            HttpResponseMessage response = await _client.GetAsync(route);

            //assert
            response.Should().BeSuccessful();


        }

        [Fact]
        public async Task UpdateTaskPost_ShouldBeSuccessful()
        {

            var testTask = new SingleTask("Test", 234);

            _dbContext.Add(testTask);
            _dbContext.SaveChanges();
            TaskCreateDTO model = new TaskCreateDTO()
            {

                Id = testTask.Id,
                HasStartTime = false,
                Name = testTask.Name,
                StartTime = testTask.StartTime,
                Time = testTask.TimeMin
            };
            HttpContent httpContent = PrepareHttpContentForUpdateTaksRequest(model);

            //act
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.UpdateTask,httpContent);

            //assert
            response.Should().BeSuccessful();


        }

        [Fact]
        public async Task AddNewTask_ShouldReturnView()
        {
            //act
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.AddNewTask);
            
            //assert
            response.Should().BeSuccessful();


        }


        [Theory]
        [ClassData(typeof(ArgumentsAddNewTaskForFalidDataTest))]
        public async Task AddNewTask_ForValidTask_StatusShouldBeCreated(TaskCreateDTO model)
        {

            
            HttpContent httpContent = PrepareHttpContentForAddNewTaksRequest(model);
            
            //act
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.AddNewTask, httpContent);
            
            
            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);


        }


        [Fact]
        public async Task AddNewTask_ForInvalidTask_StatusShouldBadRequest()
        {
            var model = new TaskCreateDTO()
            {
                Name = "Test",
                Time = -20,
                HasStartTime = true,
                StartTime = new TimeOnly(5, 25)

            };
            HttpContent httpContent = PrepareHttpContentForAddNewTaksRequest(model);

            //act
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.AddNewTask, httpContent);


            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);


        }




        [Fact]
        public async Task DeleteTask_ForValidId_StatusShouldBeOk()
        {

            var testTask = new SingleTask("Test", 234);

            _dbContext.Add(testTask);
            _dbContext.SaveChanges();



            string route = RouteConstants.DeleteTask + "?taskToDeleteId=" + testTask.Id.ToString();

            //act
            HttpResponseMessage response = await _client.GetAsync(route);



  
            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            


        }



        [Fact]
        public async Task DeleteTask_ForInvalidId_StatusShouldBeBadRequest()
        {

            var testTask = new SingleTask("Test", 234);
            _dbContext.Add(testTask);
            _dbContext.SaveChanges();
            Guid idOfNotExistingTask= new Guid("8588FE67-B618-4ED0-BCB0-B0A2290F00AE");

            string route = RouteConstants.DeleteTask + "?taskToDeleteId=" + idOfNotExistingTask.ToString();
            //act
            HttpResponseMessage response = await _client.GetAsync(route);


            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);


        }

        private static HttpContent PrepareHttpContentForAddNewTaksRequest(TaskCreateDTO model)
        {
            var contentStr = $"Name={model.Name}&Time={model.Time}&HasStartTime={model.HasStartTime}";

            if(model.HasStartTime)
                contentStr = $"{contentStr}&StartTime={model.StartTime}";

            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            return httpContent;
        }

        private static HttpContent PrepareHttpContentForUpdateTaksRequest(TaskCreateDTO model)
        {
            var contentStr = $"Name={model.Name}&Time={model.Time}&HasStartTime={model.HasStartTime}&Id={model.Id}";

            if (model.HasStartTime)
                contentStr = $"{contentStr}&StartTime={model.StartTime}";

            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            return httpContent;
        }

    }
}
