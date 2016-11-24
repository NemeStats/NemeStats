using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Facades
{
    public class RecentPublicGamesRetriever : Cacheable<RecentlyPlayedGamesFilter, List<PublicGameSummary>>, IRecentPublicGamesRetriever
    {
        public const int CACHE_EXPIRATION_IN_SECONDS = 60 * 60;

        private readonly IPlayedGameRetriever _playedGameRetriever;

        public RecentPublicGamesRetriever(
            IDateUtilities dateUtilities, 
            IPlayedGameRetriever playedGameRetriever, 
            ICacheService cacheService) 
            : base(dateUtilities, cacheService)
        {
            _playedGameRetriever = playedGameRetriever;
        }

        public override List<PublicGameSummary> GetFromSource(RecentlyPlayedGamesFilter filter)
        {
            return _playedGameRetriever.GetRecentPublicGames(filter);
        }

        public override int GetCacheExpirationInSeconds()
        {
            return CACHE_EXPIRATION_IN_SECONDS;
        }
    }
}