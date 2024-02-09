using ScheduleHelper.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class ScheduleSettingsDTO
    {
        [Required]
        [StartTimeNotLaterThanFinish]
        public TimeOnly startTime { get; set; }

        [Required]
        public TimeOnly finishTime { get; set; }

        [Required]
        public bool hasScheduledBreaks { get; set; }

        [MoreThanZero(ErrorMessage ="break duration has to be greater than 0")]
        public double breakLenghtMin { get; set; }

        
        [MoreThanZero(ErrorMessage = "Minimal time of work has to be greater than 0")]
        public double MaxWorkTimeBeforeBreakMin { get; set; }

        
        [MoreThanZero(ErrorMessage = "Max time of work has to be greater than 0")]
        public double MinWorkTimeBeforeBreakMin { get; set; }
    }
}
