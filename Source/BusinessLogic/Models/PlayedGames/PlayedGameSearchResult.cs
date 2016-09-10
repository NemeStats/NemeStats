using System;
using System.Collections.Generic;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayedGameSearchResult
    {
        public int PlayedGameId { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string Notes { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public IList<PlayerResult> PlayerGameResults { get; set; }
        public WinnerTypes WinnerType { get; set; }
        public IList<ApplicationLinkage> ApplicationLinkages { get; set; }
    }
}
