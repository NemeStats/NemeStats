using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BusinessLogic.Models.Validation
{
    public class MaxDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateTime = (DateTime)value;
            
            if (dateTime <= DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Date cannot be in the future.");
        }
    }
}