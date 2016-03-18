using System.ComponentModel.DataAnnotations;

namespace UI.Areas.Api.Models
{
    public class NewGameDefinitionMessage
    {
        [Required]
        public string GameDefinitionName { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
        public int? GamingGroupId { get; set; }
    }
}
