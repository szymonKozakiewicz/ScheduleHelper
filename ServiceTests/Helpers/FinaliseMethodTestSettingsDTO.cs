using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.Helpers
{
    public class FinaliseMethodTestSettingsDTO
    {

        public List<TimeSlotInSchedule> ListOfSlots;
        public List<TimeSlotInSchedule> ListOfExpectedSlots;
        public TimeOnly ActualFinishTime;
        public FinaliseSlotDTO Model;
    }
}
