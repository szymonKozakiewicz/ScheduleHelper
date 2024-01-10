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
        public TimeOnly startTime { get; set; }
        [Required] 
        public TimeOnly finishTime { get; set; }
        [Required]
        public bool hasScheduledBreaks { get; set; }
        [MoreThanZeroValidation]
        public double breakLenghtMin { get; set; }
    }
}
