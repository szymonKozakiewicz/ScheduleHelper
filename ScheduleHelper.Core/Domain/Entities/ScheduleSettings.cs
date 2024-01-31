using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities
{
    public class ScheduleSettings
    {
        [Key]
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly FinishTime { get; set; }
        public double breakDurationMin { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ScheduleSettings settings &&
                   StartTime.Equals(settings.StartTime) &&
                   FinishTime.Equals(settings.FinishTime) &&
                   breakDurationMin == settings.breakDurationMin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, FinishTime, breakDurationMin);
        }
    }
}
