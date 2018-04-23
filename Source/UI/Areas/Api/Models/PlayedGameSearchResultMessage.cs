using System.Collections.Generic;

namespace UI.Areas.Api.Models
{
    public class PlayedGameSearchResultMessage
    {
        public int PlayedGameId { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string Notes { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string DatePlayed { get; set; }
        public string DateLastUpdated { get; set; }
        public List<PlayerGameResultMessage> PlayerGameResults { get; set; }
        public List<ApplicationLinkageMessage> ApplicationLinkages { get; set; }
        public string NemeStatsUrl { get; set; }
        public string WinnerType { get; set; }
    }
}
