using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.Nemeses
{
    public class NemesisChangeViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int LossPercentageVersusNemesis { get; set; }
    }
}