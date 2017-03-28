using System.Collections.Generic;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementTileViewModel
    {
        public AchievementId AchievementId { get; set; }
        public string AchievementName { get; set; }
        public string IconClass { get; set; }
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Description { get; set; }
        public AchievementGroup Group { get; set; }
        public int NumberOfPlayersWithThisAchievement { get; set; }
        public bool CurrentPlayerUnlockedThisAchievement { get; set; }
    }
}