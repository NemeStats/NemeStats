using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IPlayerAchievementRetriever
    {
        PlayerAchievement GetPlayerAchievement(int playerId, AchievementId achievementId);
    }
}