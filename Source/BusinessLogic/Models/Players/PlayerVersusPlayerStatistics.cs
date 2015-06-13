using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Players
{
    public class PlayerVersusPlayerStatistics
    {
        public string OpposingPlayerName { get; set; }
        public int OpposingPlayerId { get; set; }
        public int NumberOfGamesWonVersusThisPlayer { get; set; }
        public int NumberOfGamesPlayedVersusThisPlayer { get; set; }
    }
}
