using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities.Helpers
{
    public static class TimeOnlyExtension
    {
        public static bool AreTimesEqualWithTolerance(this TimeOnly time1,TimeOnly time2)
        {
            double toleranceMinutes = 0.1;
            TimeOnly upperBound = time1.AddMinutes(toleranceMinutes);

           
            TimeOnly lowerBound = time1.AddMinutes(-toleranceMinutes);

            
            return time2.IsBetween(lowerBound,upperBound);
            
        }

        public static bool IsBetweenOpenBrackets(this TimeOnly time,TimeOnly start, TimeOnly end)
        {
            long startTicks = start.Ticks;
            long endTicks = end.Ticks;
            long ticks = time.Ticks;

            return startTicks <= endTicks
                ? (startTicks < ticks && endTicks > ticks)
                : (startTicks < ticks || endTicks > ticks);

        }
    }
}
