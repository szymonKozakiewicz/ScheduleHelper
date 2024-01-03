using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using ScheduleHelper.Core.DTO;
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
        HttpClient client;
        public TasksControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTasksListView_ShouldReturnView()
        {
            HttpResponseMessage response = await client.GetAsync(RouteConstants.ShowTasksList);
            response.Should().BeSuccessful();

            
        }

        [Fact]
        public async Task AddNewTask_ShouldReturnView()
        {
            HttpResponseMessage response = await client.GetAsync(RouteConstants.AddNewTask);
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
            HttpResponseMessage response = await client.PostAsync(RouteConstants.AddNewTask, httpContent);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);


        }
    }
}
