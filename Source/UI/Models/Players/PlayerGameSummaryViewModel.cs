using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.Players
{
    public class PlayerGameSummaryViewModel
    {
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int NumberOfGamesLost { get; set; }
        public int WinPercentage { get; set; }
        public bool IsChampion { get; set; }
    }
}
