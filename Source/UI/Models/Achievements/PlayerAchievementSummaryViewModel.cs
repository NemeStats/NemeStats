using System;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class PlayerAchievementSummaryViewModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public AchievementViewModel Achievement { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
    }
}