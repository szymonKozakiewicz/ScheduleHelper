using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ScheduleHelper.Core.Domain.Entities
{
    public class DaySchedule
    {
        [Key]
        public Guid Id { get; set; }

        public ScheduleSettings Settings { get; set; }
        public double TimeFromLastBreakMin { get; set; }

        public DaySchedule Copy()
        {
            return new DaySchedule
            {
                Id = Id,
                Settings = Settings,
                TimeFromLastBreakMin = TimeFromLastBreakMin
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is DaySchedule schedule &&
                   Id.Equals(schedule.Id) &&
                   EqualityComparer<ScheduleSettings>.Default.Equals(Settings, schedule.Settings) &&
                   TimeFromLastBreakMin == schedule.TimeFromLastBreakMin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Settings, TimeFromLastBreakMin);
        }
    }
}
