using System.Collections.Generic;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Models.GameDefinitionModels
{
    public class GamingGroupGameDefinitionViewModel
    {
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public List<GameDefinitionPlayerSummaryViewModel> GameDefinitionPlayerSummaries { get; set; }
        public string PlayedGamesPanelTitle { get; set; }
        public List<PlayedGameDetailsViewModel> PlayedGames { get; set; }
        public GamingGroupGameDefinitionStatsViewModel GamingGroupGameDefinitionStats { get; set; }
    }
}