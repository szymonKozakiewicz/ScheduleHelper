using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ScheduleHelper.Core.DTO;
using ScheduleHelper.Infrastructure;
using ScheduleHelper.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [Fact]
        public async Task GenerateScheduleSettings_shouldReturnsStatusCreated()
        {
            var model = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 24),

            };
            HttpContent httpContent = generateContentMessage(model);


            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, httpContent);


            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        private static HttpContent generateContentMessage(ScheduleSettingsDTO model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"breakLenghtMin={model.breakLenghtMin}");
            stringBuilder.Append($"&hasScheduledBreaks={model.hasScheduledBreaks}");
            stringBuilder.Append($"&startTime={model.startTime}");
            stringBuilder.Append($"&finishTime={model.finishTime}");
            var contentStr = stringBuilder.ToString();
            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            return httpContent;
        }

        [Fact]
        public async Task GenerateScheduleSettings_shouldReturnView()
        {
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.GenerateScheduleSettings);


            response.Should().BeSuccessful();
        }


        [Fact]
        public async Task GenerateScheduleSettings_forValidData_ShouldReturnStatusCreated()
        {

            var testModel = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 24),
            };

            var contetMessage=generateContentMessage(testModel);
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, contetMessage);


            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }



        [Theory]
        [MemberData(nameof(GetSampleOfInvalidDataForGenerateScheduleSettingsTest))]
        public async Task GenerateScheduleSettings_forInValidData_ShouldReturnStatusBadRequest(ScheduleSettingsDTO settingsDTO)
        {

            var contetMessage = generateContentMessage(settingsDTO);
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, contetMessage);


            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        public static IEnumerable<object[]> GetSampleOfInvalidDataForGenerateScheduleSettingsTest()
        {
            var testModel1 = new ScheduleSettingsDTO()
            {
                breakLenghtMin = -20,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 24),
            };
            yield return new object[] { testModel1 };
            var testModel2 = new ScheduleSettingsDTO()
            {
                breakLenghtMin = 0,
                hasScheduledBreaks = true,
                startTime = new TimeOnly(12, 24),
                finishTime = new TimeOnly(21, 24),
            };
            yield return new object[] { testModel2 };
        }
    }
}
