using System.Collections.Generic;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementViewModel
    {
        public AchievementId Id { get; set; }
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
    }
}