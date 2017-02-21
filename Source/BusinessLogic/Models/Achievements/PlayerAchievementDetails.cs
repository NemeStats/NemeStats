using System;
using System.Collections.Generic;

namespace BusinessLogic.Models.Achievements
{
    public class PlayerAchievementDetails
    {
        public AchievementId AchievementId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public AchievementLevel? AchievementLevel { get; set; }
        public string AchievementName { get; set; }
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementIconClass { get; set; }
        public int NumberOfPlayersWithThisAchievement { get; set; }
        public int PlayerProgress { get; set; }
        public List<AchievementRelatedGameDefinitionSummary> RelatedGameDefinitions { get; set; } = new List<AchievementRelatedGameDefinitionSummary>();
        public List<AchievementRelatedPlayedGameSummary> RelatedPlayedGames { get; set; } = new List<AchievementRelatedPlayedGameSummary>();
        public List<AchievementRelatedPlayerSummary> RelatedPlayers { get; set; } = new List<AchievementRelatedPlayerSummary>();
        public List<AchievementWinner> Winners { get; set; } = new List<AchievementWinner>();
    }
}
