using ScheduleHelper.Core.Domain.Entities;
using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.ServiceTests.Helpers.Dto
{
    public class FinaliseMethodTestSettingsDTO
    {

        public TimeSlotList ListOfSlots;
        public TimeSlotList ListOfExpectedSlots;
        public TimeOnly ActualFinishTime;
        public FinaliseSlotDTO Model;
    }
}
