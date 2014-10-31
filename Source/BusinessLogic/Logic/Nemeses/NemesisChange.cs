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
        public float LossPercentageVersusNemesis { get; set; }
        public int MinionPlayerId { get; set; }
        public string MinionPlayerName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
