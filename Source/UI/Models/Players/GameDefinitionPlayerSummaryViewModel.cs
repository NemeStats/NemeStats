using System;
using System.Collections.Generic;
using UI.Models.Badges;

namespace UI.Models.Players
{
    public class GameDefinitionPlayerSummaryViewModel
    {
        public IList<IBadgeBaseViewModel> SpecialBadgeTypes { get; set; }
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int WinPercentage { get; set; }
        public int LostPercentage { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int TotalNemePoints { get; set; }
        public float AveragePointsPerGame { get; set; }
    }
}