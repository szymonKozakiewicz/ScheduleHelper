using System.ComponentModel.DataAnnotations;

namespace ScheduleHelper.UI.Controllers
{
    public static class ValidationHelper
    {
        public static bool ValidateObject(object objectToValidate)
        {
            ValidationContext validationContext = new ValidationContext(objectToValidate);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isError = Validator.TryValidateObject(objectToValidate, validationContext, results, true);
            return isError;
        }
    }
}
