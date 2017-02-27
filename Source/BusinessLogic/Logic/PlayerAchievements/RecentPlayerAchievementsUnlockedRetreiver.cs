using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public class RecentPlayerAchievementsUnlockedRetreiver : Cacheable<GetRecentPlayerAchievementsUnlockedQuery, IPagedList<PlayerAchievementWinner>>, IRecentPlayerAchievementsUnlockedRetreiver
    {
        public const int CACHE_EXPIRATION_IN_SECONDS = 60 * 60;

        private readonly IDataContext _dataContext;
        private readonly IAchievementRetriever _achievementRetriever;

        public RecentPlayerAchievementsUnlockedRetreiver(
            IDateUtilities dateUtilities, 
            ICacheService cacheService, 
            IDataContext dataContext, 
            IAchievementRetriever achievementRetriever) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
            _achievementRetriever = achievementRetriever;
        }

        public override IPagedList<PlayerAchievementWinner> GetFromSource(GetRecentPlayerAchievementsUnlockedQuery query)
        {
            var playerAchievementWinnersQueryable =
                _dataContext.GetQueryable<PlayerAchievement>()
                    .Select(x => new PlayerAchievementWinner
                    {
                        AchievementId = x.AchievementId,
                        AchievementLastUpdateDate = x.LastUpdatedDate,
                        AchievementLevel = x.AchievementLevel,
                        GamingGroupId = x.Player.GamingGroupId,
                        GamingGroupName = x.Player.GamingGroup.Name,
                        PlayerId = x.PlayerId,
                        PlayerName = x.Player.Name,
                        UserId = x.Player.ApplicationUserId
                    })
                    .OrderByDescending(p => p.AchievementLastUpdateDate);

            var pagedList = playerAchievementWinnersQueryable.ToPagedList(query.Page, query.PageSize);

            var distinctAchievements = pagedList.GroupBy(x => x.AchievementId).ToDictionary(x => x.Key, y => _achievementRetriever.GetAchievement(y.Key));

            foreach (var playerAchievementWinner in pagedList)
            {
                var achievement = distinctAchievements[playerAchievementWinner.AchievementId];
                playerAchievementWinner.IconClass = achievement.IconClass;
                playerAchievementWinner.AchievementName = achievement.Name;
            }

            return pagedList;
        }

        public override int GetCacheExpirationInSeconds()
        {
            return CACHE_EXPIRATION_IN_SECONDS;
        }
    }
}