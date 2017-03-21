using BusinessLogic.Models.Achievements;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public interface IRecentPlayerAchievementsUnlockedRetriever
    {
        IPagedList<PlayerAchievementWinner> GetResults(GetRecentPlayerAchievementsUnlockedQuery query);
    }
}