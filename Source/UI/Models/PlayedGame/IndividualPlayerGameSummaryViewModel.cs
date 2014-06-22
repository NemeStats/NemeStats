using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.PlayedGame
{
    public class IndividualPlayerGameSummaryViewModel
    {
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }
        public int PlayedGameId { get; set; }
        public string GameName { get; set; }
        public int GameDefinitionId { get; set; }
    }
}