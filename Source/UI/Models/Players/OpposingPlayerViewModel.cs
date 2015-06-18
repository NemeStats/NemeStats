using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.Players
{
    public class OpposingPlayerViewModel
    {
        public string Name { get; set; }
        public int PlayerId { get; set; }
        public int NumberOfGamesWonVersusThisPlayer { get; set; }
        public int NumberOfGamesLostVersusThisPlayer { get; set; }
        public int WinPercentageVersusThisPlayer { get; set; }
        public bool IsNemesis { get; set; }
        public bool IsMinion { get; set; }
    }
}
