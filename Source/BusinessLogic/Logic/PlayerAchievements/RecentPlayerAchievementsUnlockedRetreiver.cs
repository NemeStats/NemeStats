using System.Data.Entity;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Paging;
using PagedList;

namespace BusinessLogic.Logic.PlayerAchievements
{
    public class RecentPlayerAchievementsUnlockedRetreiver : Cacheable<GetRecentPlayerAchievementsUnlockedQuery, IPagedList<PlayerAchievement>>, IRecentPlayerAchievementsUnlockedRetreiver
    {
        public const int CACHE_EXPIRATION_IN_SECONDS = 60 * 60;

        private readonly IDataContext _dataContext;

        public RecentPlayerAchievementsUnlockedRetreiver(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override IPagedList<PlayerAchievement> GetFromSource(GetRecentPlayerAchievementsUnlockedQuery inputParameter)
        {
            var playerAchievements =
                _dataContext.GetQueryable<PlayerAchievement>()
                    .Include(pa => pa.Player)
                    .Include(pa => pa.Player.GamingGroup)
                    .OrderByDescending(p => p.LastUpdatedDate)
                ;

            return playerAchievements.ToPagedList(inputParameter.Page, inputParameter.PageSize);
        }

        public override int GetCacheExpirationInSeconds()
        {
            return CACHE_EXPIRATION_IN_SECONDS;
        }
    }
}