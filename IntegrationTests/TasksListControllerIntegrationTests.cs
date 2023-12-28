using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ScheduleHelper.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.IntegrationTests
{
    public class TasksListControllerIntegrationTests:IClassFixture<WebApplicationFactory<Program>>
    {
        HttpClient client;
        public TasksListControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTasksListView_ShouldReturnView()
        {
            HttpResponseMessage response = await client.GetAsync("/");
            response.Should().BeSuccessful();

            
        }
    }
}
