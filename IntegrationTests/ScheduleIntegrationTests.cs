using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.Entities.Builders;
using ScheduleHelper.Core.Domain.Entities.Enums;
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
    public class ScheduleIntegrationTests : IClassFixture<InMemoryWebApplicationFactory>
    {
        HttpClient _client;
        MyDbContext _dbContext;
        IServiceScope scope;
        public ScheduleIntegrationTests(InMemoryWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            _dbContext = factory.GetDbContextInstance();

        }

        [Fact]
        public async Task ShowSchedule_shouldReturnView()
        {
            //act
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.ShowSchedule);

            //assert
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
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin = 10

            };
            HttpContent httpContent = generateContentMessage(model);


            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, httpContent);


            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }



        [Theory]
        [InlineData("2", "true", "02:12", "blablaasdasd")]
        [InlineData("2", "true", "blablaasdasd", "05:12")]
        [InlineData("2", "blablaasdasd", "02:12", "05:12")]
        [InlineData("blablaasdasd", "true", "02:12", "05:12")]
        public async Task GenerateScheduleSettings_ForNotValidData_shouldReturnsBadRequest(string breakLenghtMin, string hasScheduledBreaks, string startTime, string finishTime)
        {


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"breakLenghtMin={hasScheduledBreaks}");
            stringBuilder.Append($"&hasScheduledBreaks={hasScheduledBreaks}");
            stringBuilder.Append($"&startTime={startTime}");
            stringBuilder.Append($"&finishTime={finishTime}");
            var contentStr = stringBuilder.ToString();
            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, httpContent);


            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



        [Fact]
        public async Task GenerateScheduleSettings_shouldReturnView()
        {

            //act
            HttpResponseMessage response = await _client.GetAsync(RouteConstants.GenerateScheduleSettings);

            //assert
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
                MaxWorkTimeBeforeBreakMin = 60,
                MinWorkTimeBeforeBreakMin = 10

            };

            var contetMessage = generateContentMessage(testModel);

            //act
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, contetMessage);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }



        [Theory]
        [MemberData(nameof(GetSampleOfInvalidDataForGenerateScheduleSettingsTest))]
        public async Task GenerateScheduleSettings_forInValidData_ShouldReturnStatusBadRequest(ScheduleSettingsDTO settingsDTO)
        {

            var contetMessage = generateContentMessage(settingsDTO);

            //act
            HttpResponseMessage response = await _client.PostAsync(RouteConstants.GenerateScheduleSettings, contetMessage);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetTimeSlotFinalisePage_RequestShouldBeSuccessful()
        {
            var id = Guid.NewGuid();
            var finishTime = new TimeOnly(12, 23).ToString();
            int complitedShareOfTask = 100;
            SingleTask task = new SingleTask("test", 23);
            TimeSlotInSchedule timeSlot = await prepareDbWithTaskAndOneSlote(task);
            //act
            var result = await _client.GetAsync(RouteConstants.FinishTimeSlot + "?slotId=" + timeSlot.Id + "&finishTime=" + finishTime + "&complitedShareOfTask=" + complitedShareOfTask);


            //assert
            result.Should().BeSuccessful();
        }


        [Fact]
        public async Task ShowScheduleSettings_RequestShouldBeSuccessful()
        {
            var id = Guid.NewGuid();


            //act
            var result = await _client.GetAsync(RouteConstants.ShowScheduleSettings);


            //assert
            result.Should().BeSuccessful();
        }

        [Fact]
        public async Task TimeSlotFinalise_schouldBeSuccessful()
        {

            var finishTime = new TimeOnly(12, 23).ToString();
            SingleTask task = new SingleTask("test", 23);
            TimeSlotInSchedule timeSlot = await prepareDbWithTaskAndOneSlote(task);
            var model = new FinaliseSlotDTO()
            {
                SlotId = (Guid)timeSlot.Id,
                FinishTime = finishTime
            };
            var contentMessage = generateContentMessage(model);

            //act
            var result = await _client.PostAsync(RouteConstants.FinishTimeSlot, contentMessage);


            //assert
            result.Should().BeSuccessful();
        }

        private async Task<TimeSlotInSchedule> prepareDbWithTaskAndOneSlote(SingleTask task)
        {
            var settings = new ScheduleSettings()
            {
                breakDurationMin = 20,
                FinishTime = new TimeOnly(23, 59),
                StartTime = new TimeOnly(2, 0)

            };
            var timeSlot = new TimeSlotInScheduleBuilder()
                .SetFinishTime(new TimeOnly(20, 0))
                .SetStartTime(new TimeOnly(19, 0))
                .SetTask(task)
                .SetOrdinalNumber(1)
                .SetTimeSlotStatus(TimeSlotStatus.Active)
                .Build();
            var daySchedule = new DaySchedule()
            {
                TimeFromLastBreakMin = 0,
                Settings = settings
            };
            _dbContext.SingleTask.Add(task);
            await _dbContext.SaveChangesAsync();
            _dbContext.TimeSlotsInSchedule.Add(timeSlot);
            await _dbContext.SaveChangesAsync();
            _dbContext.ScheduleSettings.Add(settings);
            await _dbContext.SaveChangesAsync();
            _dbContext.DaySchedule.Add(daySchedule);
            await _dbContext.SaveChangesAsync();
            return timeSlot;
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
        private void clearDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }
        private static HttpContent generateContentMessage(ScheduleSettingsDTO model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"breakLenghtMin={model.breakLenghtMin}");
            stringBuilder.Append($"&hasScheduledBreaks={model.hasScheduledBreaks}");
            stringBuilder.Append($"&startTime={model.startTime}");
            stringBuilder.Append($"&finishTime={model.finishTime}");
            stringBuilder.Append($"&MaxWorkTimeBeforeBreakMin={model.MaxWorkTimeBeforeBreakMin}");
            stringBuilder.Append($"&MinWorkTimeBeforeBreakMin={model.MinWorkTimeBeforeBreakMin}");
            var contentStr = stringBuilder.ToString();
            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            return httpContent;
        }


        private static HttpContent generateContentMessage(FinaliseSlotDTO model)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"slotId={model.SlotId}");
            stringBuilder.Append($"&finishTime={model.FinishTime}");

            var contentStr = stringBuilder.ToString();
            HttpContent httpContent = new StringContent(contentStr, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            return httpContent;
        }
    }
}
