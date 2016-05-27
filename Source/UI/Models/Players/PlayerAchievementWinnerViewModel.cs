using System;
using BusinessLogic.Models.Achievements;
using UI.Models.Achievements;

namespace UI.Models.Players
{
    public class PlayerAchievementWinnerViewModel
    {
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public string GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public DateTime AchievementLastUpdateDate { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
        public AchievementSummaryViewModel Achievement { get; set; }
        
    }
}