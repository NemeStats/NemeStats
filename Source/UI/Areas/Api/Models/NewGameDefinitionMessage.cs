using System.ComponentModel.DataAnnotations;

namespace UI.Areas.Api.Models
{
    public class NewGameDefinitionMessage
    {
        [Required]
        public string GameDefinitionName { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
    }
}
