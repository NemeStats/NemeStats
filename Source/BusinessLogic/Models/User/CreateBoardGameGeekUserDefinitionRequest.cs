using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models.Validation;

namespace BusinessLogic.Models.User
{
    public class CreateBoardGameGeekUserDefinitionRequest : ValidatableRequest
    {
        [Required]
        public string Name { get; set; }

        public string Avatar { get; set; }
    }
}