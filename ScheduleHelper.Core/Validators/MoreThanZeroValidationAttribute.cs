using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleHelper.Core.Validators
{
    public class MoreThanZeroValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            double numberToValidate = (double)value;
            if (numberToValidate > 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}
