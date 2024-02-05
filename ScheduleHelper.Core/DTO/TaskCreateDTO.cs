using ScheduleHelper.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class TaskCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [MoreThanZero(ErrorMessage ="Time has to be greater than 0")]
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
