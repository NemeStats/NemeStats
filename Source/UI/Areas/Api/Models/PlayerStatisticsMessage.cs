using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class PlayerStatisticsMessage
    {
        public int TotalGames { get; set; }
        public int TotalPoints { get; set; }
        public float AveragePlayersPerGame { get; set; }
        public int TotalGamesWon { get; set; }
        public int TotalGamesLost { get; set; }
        public int WinPercentage { get; set; }
        public GameDefinitionTotalsMessage GameDefinitionTotals { get; set; }
    }
}
