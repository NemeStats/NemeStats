using System;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class PlayerAchievementSummaryViewModel
    {
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public AchievementLevel? AchievementLevel { get; set; }

        public bool HasAnyLevelUnlocked => AchievementLevel.HasValue && AchievementLevel.Value != BusinessLogic.Models.Achievements.AchievementLevel.None && DateCreated.HasValue && DateCreated.Value > DateTime.MinValue;
        public AchievementId AchievementId { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public string AchievementName { get; set; }
    }
}