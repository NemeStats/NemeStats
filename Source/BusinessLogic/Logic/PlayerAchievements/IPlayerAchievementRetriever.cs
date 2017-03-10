using System;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IPlayerAchievementRetriever
    {
        [Obsolete("going away soon")]
        PlayerAchievement GetPlayerAchievement(int playerId, AchievementId achievementId);
        PlayerAchievementDetails GetCurrentPlayerAchievementDetails(AchievementId achievementId, ApplicationUser currentUser);
        PlayerAchievementDetails GetPlayerAchievement(PlayerAchievementQuery playerAchievementQuery);
    }
}