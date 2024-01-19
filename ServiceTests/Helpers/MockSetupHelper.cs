using Moq;
using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.Helpers
{
    public static class MockSetupHelper
    {
        public static void SetupMockForGetTimeSlotsListMethodForGivenTimeSlotList(List<TimeSlotInSchedule> timeSlotsList, Mock<IScheduleRepository> scheduleRespositorMock)
        {
            scheduleRespositorMock.Setup(m => m.GetTimeSlotsList()).ReturnsAsync(timeSlotsList);
        }
    }
}
