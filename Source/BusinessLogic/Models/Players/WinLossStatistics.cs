using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Players
{
    public class WinLossStatistics
    {
        public int NumberOfGamesLost { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int VersusPlayerId { get; set; }
    }
}
