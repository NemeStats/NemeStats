using System;
using System.Linq;

namespace UI.Areas.Api.Models
{
    public class GameDefinitionSearchResultMessage
    {
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public bool Active { get; set; }
        [Obsolete("The API documentation references BoardGameGeekObjectId and not BoardGameGeekGameDefinitionId. This field will go away in /api/v3")]
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
        public string NemeStatsUrl { get; set; }
    }
}
