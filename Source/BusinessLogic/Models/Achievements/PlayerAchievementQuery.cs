namespace BusinessLogic.Models.Achievements
{
    public class PlayerAchievementQuery
    {
        public AchievementId AchievementId { get; }
        public int PlayerId { get; }

        public PlayerAchievementQuery(AchievementId achievementId, int playerId)
        {
            AchievementId = achievementId;
            PlayerId = playerId;
        }
    }
}
