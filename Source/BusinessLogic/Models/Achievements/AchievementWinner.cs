using System;

namespace BusinessLogic.Models.Achievements
{
    public class AchievementWinner
    {
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public DateTime AchievementLastUpdateDate { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
    }
}
