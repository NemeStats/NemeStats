using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.PlayedGame
{
    public class GameResultViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }
        public int PlayedGameId { get; set; }
        public string GameName { get; set; }
        public int GameDefinitionId { get; set; }
    }
}
