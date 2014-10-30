using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.Nemeses
{
    public class NemesisChange
    {
        public int NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int LossPercentageVersusNemesis { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
    }
}
