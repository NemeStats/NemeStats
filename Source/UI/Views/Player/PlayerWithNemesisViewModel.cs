using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Views.Player
{
    public class PlayerWithNemesisViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int? NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
    }
}
