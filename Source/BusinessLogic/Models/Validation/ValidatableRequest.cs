using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models.Validation
{
    public abstract class ValidatableRequest : IValidatableRequest
    {
        public List<ValidationResult> ValidateRequest()
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this), results);

            return results;
        }

    }

    public interface IValidatableRequest
    {
        List<ValidationResult> ValidateRequest();
    }
}
