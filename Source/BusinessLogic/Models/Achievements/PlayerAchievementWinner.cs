using System;

namespace BusinessLogic.Models.Achievements
{
    public class PlayerAchievementWinner
    {
        public AchievementId AchievementId { get; set; }
        public string AchievementName { get; set; }
        public string IconClass { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Description { get; set; }
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public string UserId { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public DateTime AchievementLastUpdateDate { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
    }
}
