using ScheduleHelper.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.DTO
{
    public class ScheduleSettingsPostDTO
    {
        [Required]
        public string startTime { get; set; }
        [Required]
        public string finishTime { get; set; }
        [Required]
        public bool hasScheduledBreaks { get; set; }
        [MoreThanZeroValidation]
        public double breakLenghtMin { get; set; }


        public ScheduleSettingsDTO ConvertToScheduleSettingsDTO()
        {
            return new ScheduleSettingsDTO
            {

                startTime = TimeOnly.Parse(this.startTime),
                finishTime = TimeOnly.Parse(this.finishTime),
                hasScheduledBreaks = this.hasScheduledBreaks,
                breakLenghtMin = this.breakLenghtMin


            };
        }
    }
}
