using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BusinessLogic.Models.Games.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class PlayerRankValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<PlayerRank> playerRanks = value as List<PlayerRank>;

            try
            {
                PlayerRankValidator.ValidatePlayerRanks(playerRanks);

                return ValidationResult.Success;
            }catch(ArgumentException argumentException)
            {
                return new ValidationResult(argumentException.Message);
            }
        }
    }
}
