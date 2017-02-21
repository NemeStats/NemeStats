using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IPlayerAchievementRetriever
    {
        PlayerAchievement GetPlayerAchievement(int playerId, AchievementId achievementId);
        PlayerAchievementDetails GetCurrentPlayerAchievementDetails(AchievementId achievementId, ApplicationUser currentUser);
    }
}