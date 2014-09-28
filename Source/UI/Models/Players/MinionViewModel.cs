using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.Players
{
    public class MinionViewModel
    {
        public string MinionName { get; set; }
        public int MinionPlayerId { get; set; }
        public int NumberOfGamesWonVersusMinion { get; set; }
        public int WinPercentageVersusMinion { get; set; }
    }
}
