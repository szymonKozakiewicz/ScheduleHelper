using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class TaskCreateDTO
    {

        public string Name { get; set; }
        public double Time { get; set; }

        public TaskCreateDTO() {
            Name = "";
            Time = 1;
        }

        public override bool Equals(object? obj)
        {
            return obj is TaskCreateDTO dTO &&
                   Name == dTO.Name &&
                   Time == dTO.Time;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Time);
        }
    }
}
