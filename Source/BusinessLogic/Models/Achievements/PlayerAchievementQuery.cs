namespace BusinessLogic.Models.Achievements
{
    public class PlayerAchievementQuery
    {
        public AchievementId AchievementId { get; }
        public int? PlayerId { get; }
        public string ApplicationUserId { get; }
        public int? GamingGroupId { get; }

        public PlayerAchievementQuery(AchievementId achievementId, int playerId)
        {
            AchievementId = achievementId;
            PlayerId = playerId;
        }

        public PlayerAchievementQuery(AchievementId achievementId, string applicationUserId, int gamingGroupId)
        {
            AchievementId = achievementId;
            ApplicationUserId = applicationUserId;
            GamingGroupId = gamingGroupId;
        }
    }
}
