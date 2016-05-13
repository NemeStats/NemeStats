using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public class RecentPublicGamesRetriever : Cacheable<int, List<PublicGameSummary>>, IRecentPublicGamesRetriever
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

        public override List<PublicGameSummary> GetFromSource(int numberOfGamesToRetrieve)
        {
            return _playedGameRetriever.GetRecentPublicGames(numberOfGamesToRetrieve);
        }

        public override int GetCacheExpirationInSeconds()
        {
            return CACHE_EXPIRATION_IN_SECONDS;
        }
    }
}