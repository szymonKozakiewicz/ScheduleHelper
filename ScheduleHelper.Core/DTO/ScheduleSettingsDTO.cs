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
        
        public TimeOnly startTime { get; set; }
        
        public TimeOnly finishTime { get; set; }
        
        public bool hasScheduledBreaks { get; set; }
        
        public double breakLenghtMin { get; set; }
    }
}
