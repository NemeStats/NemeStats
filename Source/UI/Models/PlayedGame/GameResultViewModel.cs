using System;
using System.Linq;

namespace UI.Models.PlayedGame
{
    public class GameResultViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }
        public int PlayedGameId { get; set; }
        public DateTime DatePlayed { get; set; }
        public string GameName { get; set; }
        public int GameDefinitionId { get; set; }
    }
}
