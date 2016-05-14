using System;
using System.Collections.Generic;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementViewModel
    {
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
    }

    public class PlayerAchievementViewModel
    {
        public AchievementViewModel Achievement { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public List<int> RelatedEntities { get; set; }

    }
}