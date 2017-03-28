using System;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Players
{
    public class PlayerAchievementWinnerViewModel
    {
        public AchievementId AchievementId { get; set; }
        public string AchievementName { get; set; }
        public string IconClass { get; set; }
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public string UserId { get; set; }
        public string GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public DateTime AchievementLastUpdateDate { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
    }
}