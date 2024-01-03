using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class SingleTaskDTO
    {

        public string Name { get; set; }
        public double Time { get; set; }
        public SingleTaskDTO() {
            Name = "";
            Time = 1;
        }

        public override bool Equals(object? obj)
        {
            return obj is SingleTaskDTO dTO &&
                   Name == dTO.Name &&
                   Time == dTO.Time;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Time);
        }
    }
}
