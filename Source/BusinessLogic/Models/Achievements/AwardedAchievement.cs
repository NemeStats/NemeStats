namespace BusinessLogic.Models.Achievements
{
    public class AwardedAchievement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FontAwesomeIcon { get; set; }
        public AchievementLevelEnum AchievementLevel { get; set; }
    }
}
