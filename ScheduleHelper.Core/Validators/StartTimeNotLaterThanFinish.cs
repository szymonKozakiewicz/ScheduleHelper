using ScheduleHelper.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Validators
{
    public class StartTimeNotLaterThanFinish : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var instance=(ScheduleSettingsDTO)validationContext.ObjectInstance;
            
            if (instance.startTime<instance.finishTime) {
                return ValidationResult.Success; 
            
            }
            else
            {
                return new ValidationResult("start time can't be later than finish time");
            }
        }
    }
}
