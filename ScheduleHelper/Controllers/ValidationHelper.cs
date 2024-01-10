using System.ComponentModel.DataAnnotations;

namespace ScheduleHelper.UI.Controllers
{
    public static class ValidationHelper
    {
        public static bool HasObjectGotValidationErrors(object objectToValidate)
        {
            if(objectToValidate == null)
                return false;
            ValidationContext validationContext = new ValidationContext(objectToValidate);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isCorrect = Validator.TryValidateObject(objectToValidate, validationContext, results, true);
            return !isCorrect;
        }
    }
}
