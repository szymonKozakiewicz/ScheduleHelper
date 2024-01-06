using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ScheduleHelper.Infrastructure;
using ScheduleHelper.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScheduleHelper.IntegrationTests
{
    public class ScheduleIntegrationTests:IClassFixture<InMemoryWebApplicationFactory>
    {
        HttpClient _client;
        MyDbContext _dbContext;
        IServiceScope scope;
        public ScheduleIntegrationTests(InMemoryWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            scope = factory.Services.CreateScope();


            _dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        }

        [Fact]
        public async Task ShowSchedule_shouldReturnView()
        {
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.ShowSchedule);
            
            
            response.Should().BeSuccessful();
        }

    }
}
