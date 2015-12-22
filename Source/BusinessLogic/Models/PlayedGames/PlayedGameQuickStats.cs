using System;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayedGameQuickStats
    {
        public Uri BoardGameGeekUri { get; set; }
        public DateTime DatePlayed { get; set; }
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public int PlayedGameId { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public WinnerTypes WinnerType { get; set; }
        public int WinningPlayerId { get; set; }
        public string WinningPlayerName { get; set; }
    }
}
