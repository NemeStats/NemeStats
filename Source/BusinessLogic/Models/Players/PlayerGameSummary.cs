using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Players
{
    public class PlayerGameSummary
    {
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
    }
}
