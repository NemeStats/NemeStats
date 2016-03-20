using System;
using System.Collections.Generic;
using UI.Models.Badges;
using UI.Models.Points;

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
        public int LostPercentage => 100 - WinPercentage;
        public int TotalGamesPlayed { get; set; }
        public NemePointsSummaryViewModel NemePointsSummary { get; set; }
        public float AveragePointsPerGame { get; set; }
    }
}