using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Infrastructure;
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
    public class TasksControllerIntegrationTests:IClassFixture<WebApplicationFactory<Program>>
    {
        HttpClient _client;
        MyDbContext _dbContext;
        IServiceScope scope;
        public TasksControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            scope = factory.Services.CreateScope();
            
               
            _dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            
        }

        [Fact]
        public async Task GetTasksListView_ShouldReturnView()
        {
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.ShowTasksList);
            response.Should().BeSuccessful();

            
        }

        [Fact]
        public async Task AddNewTask_ShouldReturnView()
        {
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.AddNewTask);
            response.Should().BeSuccessful();


        }


        [Fact]
        public async Task AddNewTask_ForValidTask_StatusShouldBeCreated()
        {
            var model = new TaskCreateDTO()
            {
                Name= "Test",
                Time=30

            };
            var contentStr = $"Name={model.Name}&Time={model.Time}";
            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.AddNewTask, httpContent);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);


        }


        [Fact]
        public async Task DeleteTask_ForValidId_StatusShouldBeNoContent()
        {
            
            var testTask = new SingleTask("Test", 234);
            _dbContext.Add(testTask);
            _dbContext.SaveChanges();

            string route=RouteConstants.DeleteTask+ "?taskToDeleteId=" + testTask.Id.ToString();
            var result=_dbContext.SingleTask.Find(testTask.Id);
            HttpResponseMessage response = await _client.GetAsync(route);


            //assert
            result.Should().NotBeNull();//making sure that object was part of db
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);


        }

    }
}
