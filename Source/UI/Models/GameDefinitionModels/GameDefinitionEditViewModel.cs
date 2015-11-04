using System.ComponentModel.DataAnnotations;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionEditViewModel
    {
        public int GameDefinitionId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
    }
}