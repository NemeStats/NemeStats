using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Facades
{
    public class TrendingGamesRetriever : Cacheable<TrendingGamesRequest, IList<TrendingGame>>, ITrendingGamesRetriever
    {
        private readonly IGameDefinitionRepository _gameDefinitionRepository;

        public TrendingGamesRetriever(
            IGameDefinitionRepository gameDefinitionRepository,
            IDateUtilities dateUtilities, 
            ICacheService cacheService) : base(dateUtilities, cacheService)
        {
            _gameDefinitionRepository = gameDefinitionRepository;
        }

        public override IList<TrendingGame> GetFromSource(TrendingGamesRequest trendingGamesRequest)
        {
            return _gameDefinitionRepository.GetTrendingGames(trendingGamesRequest.NumberOfTrendingGamesToShow, trendingGamesRequest.NumberOfDaysOfTrendingGames);
        }
    }
}