using ScheduleHelper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Services.Helpers
{
    public static class TimeSlotOverlapsTools
    {
        public static bool DoTimeSlotsOverlaps(TimeSlotInSchedule slot1, TimeSlotInSchedule slot2)
        {
            if (slot1.TestedTimeSlotIsInsideOfTimeSlot(slot2) || slot2.TestedTimeSlotIsInsideOfTimeSlot(slot1))
                return true;
            else
                return false;
        }
    }
}
