using System;
using System.Collections.Generic;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Models.Achievements
{
    public class PlayerAchievementViewModel : PlayerAchievementSummaryViewModel
    {
        public int PlayerProgress { get; set; }
        public List<GameDefinitionSummaryListViewModel> RelatedGameDefintions { get; set; } = new List<GameDefinitionSummaryListViewModel>();
        public List<PlayedGameQuickStatsViewModel> RelatedPlayedGames { get; set; } = new List<PlayedGameQuickStatsViewModel>();
        public List<PlayerListSummaryViewModel> RelatedPlayers { get; set; } = new List<PlayerListSummaryViewModel>();
    }
}