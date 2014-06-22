using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.PlayedGame
{
    public class PlayerGameResultDetailsViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }
    }
}
