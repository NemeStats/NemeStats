using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IPlayerAchievementRetriever
    {
        PlayerAchievement GetPlayerAchievement(int playerId, AchievementId achievementId);
        IPagedList<PlayerAchievement> GetRecentPlayerAchievementsUnlocked(PagedQuery query);
    }
}