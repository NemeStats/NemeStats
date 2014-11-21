using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Games;
using UI.Models.PlayedGame;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionDetailsViewModel : GameDefinitionSummaryViewModel
    {
        public IList<PlayedGameDetailsViewModel> PlayedGames { get; set; }
        public string ChampionName { get; set; }
        public int? ChampionPlayerId { get; set; }
        public int? NumberOfWins { get; set; }
        public int? NumberOfGamesPlayed { get; set; }
        public float? WinPercentage { get; set; }
        public string PreviousChampionName { get; set; }
        public int? PreviousChampionPlayerId { get; set; }
    }
}