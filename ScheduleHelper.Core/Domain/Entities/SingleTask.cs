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
        
        public bool HasStartTime { get; set; }

        public TimeOnly StartTime;

        public SingleTask()
        {
        }

        public SingleTask(string name, double timeMin)
        {
            Name = name;
            TimeMin = timeMin;
            HasStartTime = false;
            StartTime = new TimeOnly(1,2);

        }

    

        public SingleTask Copy()
        {
            var newTask = new SingleTask(Name, TimeMin);
            newTask.Id= Id;
            newTask.StartTime= StartTime;   
            newTask.HasStartTime= HasStartTime;
            return newTask;
            
        }

        public override bool Equals(object? obj)
        {
            return obj is SingleTask task &&
                   Name == task.Name &&
                   TimeMin == task.TimeMin &&
                   HasStartTime == task.HasStartTime &&
                   StartTime.Equals(task.StartTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, TimeMin, HasStartTime, StartTime);
        }
    }
}
