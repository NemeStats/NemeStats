using System.Linq;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayedGameFilter
    {
        public string StartDateGameLastUpdated { get; set; }
        public string EndDateGameLastUpdated { get; set; }
        public int? MaximumNumberOfResults { get; set; }
        public int? GamingGroupId { get; set; }
        public int? GameDefinitionId { get; set; }
        public int? PlayerId { get; set; }
        public string DatePlayedFrom { get; set; }
        public string DatePlayedTo { get; set; }
        public string ExclusionExternalSourceApplicationName { get; set; }
    }
}
