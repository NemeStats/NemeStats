using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Players
{
    public class PlayerWithNemesis
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool PlayerRegistered { get; set; }
        public int? NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int? PreviousNemesisPlayerId { get; set; }
        public string PreviousNemesisPlayerName { get; set; }
        public int GamingGroupId { get; set; }
    }
}
