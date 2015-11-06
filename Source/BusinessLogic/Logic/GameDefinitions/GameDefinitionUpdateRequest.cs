using System.Linq;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionUpdateRequest
    {
        public int GameDefinitionId { get; set; }
        public bool? Active { get; set; }
        public string Name { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string Description { get; set; }
    }
}
