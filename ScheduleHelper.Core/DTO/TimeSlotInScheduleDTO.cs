using ScheduleHelper.Core.Domain.Entities.Enums;
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
        public Guid Id { get; set; }
        public TimeOnly FinishTime { get; set; }
        public TimeOnly StartTime { get; set; }
        public int OrdinalNumber { get; set; }

        public TimeSlotStatus Status { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is TimeSlotInScheduleDTO dTO &&
                   Name == dTO.Name &&
                   FinishTime.Equals(dTO.FinishTime) &&
                   StartTime.Equals(dTO.StartTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id, FinishTime, StartTime, OrdinalNumber);
        }
    }
}
