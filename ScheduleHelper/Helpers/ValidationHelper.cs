using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ScheduleHelper.UI.Helpers
{
    public static class ValidationHelper
    {
        public static bool HasObjectGotValidationErrors(object objectToValidate)
        {
            if (objectToValidate == null)
                return true;
            ValidationContext validationContext = new ValidationContext(objectToValidate);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isCorrect = Validator.TryValidateObject(objectToValidate, validationContext, results, true);
            return !isCorrect;
        }

        public static List<string> GetErrorsList(ModelStateDictionary modelState)
        {
            List<string> errors = new List<string>();
            foreach (var value in modelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);

                }

            }
            return errors;
        }
    }
}
