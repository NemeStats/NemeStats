using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IPlayerAchievementRetriever
    {
        PlayerAchievementDetails GetPlayerAchievement(PlayerAchievementQuery playerAchievementQuery);
    }
}