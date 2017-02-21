using System;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementWinnerViewModel
    {
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public DateTime AchievementLastUpdateDate { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
    }
}