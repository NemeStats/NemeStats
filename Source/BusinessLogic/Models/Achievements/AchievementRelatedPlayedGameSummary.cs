using System;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Models.Achievements
{
    public class AchievementRelatedPlayedGameSummary
    {
        public int GameDefinitionId { get; set; }
        public int PlayedGameId { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string GameDefinitionName { get; set; }
        public DateTime DatePlayed { get; set; }
        public WinnerTypes WinnerType { get; set; }
        public int WinningPlayerId { get; set; }
        public string WinningPlayerName { get; set; }
    }
}
