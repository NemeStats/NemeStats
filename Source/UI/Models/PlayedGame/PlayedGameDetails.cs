using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.PlayedGame
{
    public class PlayedGameDetails
    {
        public int PlayedGameId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public DateTime DatePlayed { get; set; }
        public IList<PlayerGameResultDetails> PlayerResults { get; set; }
    }
}