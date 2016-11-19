﻿using System.Collections.Generic;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Models.GameDefinitionModels
{
    public class GamingGroupGameDefinitionViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public int AveragePlayersPerGame { get; set; }
        public GameDefinitionPlayersSummaryViewModel GameDefinitionPlayersSummary { get; set; }
        public string PlayedGamesPanelTitle { get; set; }
        public IEnumerable<PlayedGameDetailsViewModel> PlayedGames { get; set; }

    }
}