using System.Collections.Generic;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementTileViewModel : AchievementSummaryViewModel
    {
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Description { get; set; }
        public AchievementGroup Group { get; set; }
        public int NumberOfPlayersWithThisAchievement { get; set; }
        public bool CurrentPlayerUnlockedThisAchievement { get; set; }
    }
}