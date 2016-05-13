namespace BusinessLogic.Models.Achievements
{
    public class AwardedAchievement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FontAwesomeIcon { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
        public string Notes { get; set; }
        public int AchievementLevel1Threshold { get; set; }
        public int AchievementLevel2Threshold { get; set; }
        public int AchievementLevel3Threshold { get; set; }

    }
}
