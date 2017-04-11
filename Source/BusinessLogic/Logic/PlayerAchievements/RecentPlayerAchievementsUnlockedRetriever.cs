using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public class RecentPlayerAchievementsUnlockedRetriever : IRecentPlayerAchievementsUnlockedRetriever
    {
        private readonly IDataContext _dataContext;
        private readonly IAchievementRetriever _achievementRetriever;

        public RecentPlayerAchievementsUnlockedRetriever(
            IDataContext dataContext, 
            IAchievementRetriever achievementRetriever)
        {
            _dataContext = dataContext;
            _achievementRetriever = achievementRetriever;
        }

        public IPagedList<PlayerAchievementWinner> GetResults(GetRecentPlayerAchievementsUnlockedQuery query)
        {
            var playerAchievementWinnersQueryable =
                _dataContext.GetQueryable<PlayerAchievement>()
                    .Where(x => query.PlayerId == null || x.PlayerId == query.PlayerId.Value)
                    .Select(x => new PlayerAchievementWinner
                    {
                        AchievementId = x.AchievementId,
                        AchievementLastUpdateDate = x.LastUpdatedDate,
                        AchievementLevel = x.AchievementLevel,
                        GamingGroupId = x.Player.GamingGroupId,
                        GamingGroupName = x.Player.GamingGroup.Name,
                        PlayerId = x.PlayerId,
                        PlayerName = x.Player.Name,
                        UserId = x.Player.ApplicationUserId,
                        DateCreated = x.DateCreated,
                        LastUpdatedDate = x.LastUpdatedDate
                    })
                    .OrderByDescending(p => p.AchievementLastUpdateDate);

            var pagedList = playerAchievementWinnersQueryable.ToPagedList(query.Page, query.PageSize);

            var distinctAchievements = pagedList.GroupBy(x => x.AchievementId).ToDictionary(x => x.Key, y => _achievementRetriever.GetAchievement(y.Key));

            foreach (var playerAchievementWinner in pagedList)
            {
                var achievement = distinctAchievements[playerAchievementWinner.AchievementId];
                playerAchievementWinner.IconClass = achievement.IconClass;
                playerAchievementWinner.AchievementName = achievement.Name;
                playerAchievementWinner.Description = achievement.Description;
            }

            return pagedList;
        }
    }
}