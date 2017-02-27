using System.Collections.Generic;

namespace BusinessLogic.Models.Achievements
{
    public class AchievementSummary
    {
        public AchievementId AchievementId { get; set; }
        public string AchievementName { get; set; }
        public string IconClass { get; set; }
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Description { get; set; }
        public AchievementGroup Group { get; set; }
    }

    public class AggregateAchievementSummary : AchievementSummary
    {
        public int NumberOfPlayersWithThisAchievement { get; set; }
        public bool CurrentPlayerUnlockedThisAchievement { get; set; }
    }
}
