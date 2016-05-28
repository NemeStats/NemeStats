using BusinessLogic.Models;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IRecentPlayerAchievementsUnlockedRetreiver
    {
        IPagedList<PlayerAchievement> GetResults(GetRecentPlayerAchievementsUnlockedQuery query);
    }
}