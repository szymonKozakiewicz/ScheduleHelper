using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers.DTO
{
    public class UpdateSlotsAfterNotFirstFinishedDTO
    {
        public ScheduleSettings ScheduleSettings;
        public List<TimeSlotInSchedule> TimeSlotsList;
        public double Dely;
    }
}
