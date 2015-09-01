using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Players
{
    public class GameDefinitionTotal
    {
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
    }
}
