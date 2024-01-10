using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class TimeSlotInScheduleDTO
    {
        public string Name { get; set; }
        public double Time { get; set; }
        public Guid Id { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TimeSlotInScheduleDTO dTO &&
                   Name == dTO.Name &&
                   Time == dTO.Time &&
                   Id.Equals(dTO.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Time, Id);
        }
    }
}
