using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayedGameSearchResult
    {
        public int PlayedGameId { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
