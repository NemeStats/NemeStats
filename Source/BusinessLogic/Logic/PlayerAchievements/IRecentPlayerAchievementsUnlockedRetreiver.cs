using BusinessLogic.Models.Achievements;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IRecentPlayerAchievementsUnlockedRetreiver
    {
        IPagedList<PlayerAchievementWinner> GetResults(GetRecentPlayerAchievementsUnlockedQuery query);
    }
}