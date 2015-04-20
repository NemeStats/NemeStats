using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class PlayedGameSearchResultMessage
    {
        public int PlayedGameId { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public List<PlayerGameResultMessage> PlayerGameResults;
    }
}
