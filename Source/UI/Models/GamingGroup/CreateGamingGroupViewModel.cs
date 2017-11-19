using System.ComponentModel.DataAnnotations;

namespace UI.Models.GamingGroup
{
    public class CreateGamingGroupViewModel
    {
        [Required( ErrorMessage = "The Gaming Group name field is required.")]
        public string GamingGroupName { get; set; }
    }
}