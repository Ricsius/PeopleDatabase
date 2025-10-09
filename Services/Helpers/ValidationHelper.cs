using System.ComponentModel.DataAnnotations;

namespace Services.Helpers
{
    public class ValidationHelper
    {
        internal static void ModelValidation(object? model) 
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ValidationContext context = new ValidationContext(model);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isRequestValid = Validator.TryValidateObject(model, context, validationResults, true);

            if (!isRequestValid)
            {
                string? errorMessage = validationResults.FirstOrDefault()?.ErrorMessage;

                throw new ArgumentException(errorMessage);
            }
        }
    }
}
