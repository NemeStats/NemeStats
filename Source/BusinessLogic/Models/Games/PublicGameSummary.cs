using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Games
{
    public class PublicGameSummary
    {
        public int PlayedGameId { get; set; }
        public DateTime DatePlayed { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public Player WinningPlayer { get; set; }
    }
}
