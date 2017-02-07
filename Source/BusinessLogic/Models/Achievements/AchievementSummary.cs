using System.Collections.Generic;

namespace BusinessLogic.Models.Achievements
{
    public class AchievementSummary
    {
        public AchievementId Id { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Description { get; set; }
        public AchievementGroup Group { get; set; }
        public int NumberOfPlayersWithThisAchievement { get; set; }
        public bool CurrentPlayerUnlockedThisAchievement { get; set; }
    }
}
