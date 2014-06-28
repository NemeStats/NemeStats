using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Players
{
    public class Nemesis
    {
        public int NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int LossPercentageVersusNemesis { get; set; }
        public int GamesLostVersusNemesis { get; set; }
    }
}
