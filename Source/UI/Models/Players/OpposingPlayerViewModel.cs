using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.Players
{
    public class OpposingPlayerViewModel
    {
        public string GraphichURL { get; set; }
        public string GraphicExplanation { get; set; }
        public string Name { get; set; }
        public int PlayerId { get; set; }
        public int NumberOfGamesWonVersusThisPlayer { get; set; }
        public int WinPercentageVersusThisPlayer { get; set; }
    }
}
