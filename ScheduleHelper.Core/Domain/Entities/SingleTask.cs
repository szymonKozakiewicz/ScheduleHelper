using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Domain.Entities
{


    public class SingleTask
    {
        public string Name { get; set; }
        public double TimeMin { get; set; }
        [Key]
        public Guid Id { get; set; }

        public SingleTask(string name, double timeMin)
        {
            Name = name;
            TimeMin = timeMin;
        }

        public override bool Equals(object? obj)
        {
            return obj is SingleTask task &&
                   Name == task.Name &&
                   TimeMin == task.TimeMin;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, TimeMin);
        }
    }
}
