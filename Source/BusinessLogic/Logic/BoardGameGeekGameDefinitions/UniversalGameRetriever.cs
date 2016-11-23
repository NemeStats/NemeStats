using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class UniversalGameRetriever : IUniversalGameRetriever
    {
        public const int DEFAULT_NUMBER_OF_GAMES = 5;

        private readonly ICacheableGameDataRetriever _cacheableGameDataRetriever;
        private readonly ITransformer _transformer;
        private readonly IGameDefinitionRetriever _gameDefinitionRetriever;
        private readonly IDataContext _dataContext;

        public UniversalGameRetriever(
            ICacheableGameDataRetriever cacheableGameDataRetriever, 
            ITransformer transformer, 
            IGameDefinitionRetriever gameDefinitionRetriever, 
            IDataContext dataContext)
        {
            _cacheableGameDataRetriever = cacheableGameDataRetriever;
            _transformer = transformer;
            _gameDefinitionRetriever = gameDefinitionRetriever;
            _dataContext = dataContext;
        }

        public BoardGameGeekGameSummary GetBoardGameGeekGameSummary(int boardGameGeekGameDefinitionId, ApplicationUser currentUser, int numberOfRecentlyPlayedGamesToShow = DEFAULT_NUMBER_OF_GAMES)
        {
            var universalData = _cacheableGameDataRetriever.GetResults(boardGameGeekGameDefinitionId);
            var summary = _transformer.Transform<BoardGameGeekGameSummary>(universalData);

            var gameDefinitionId = _dataContext.GetQueryable<GameDefinition>().Where(
                    x => x.BoardGameGeekGameDefinitionId == boardGameGeekGameDefinitionId
                         && x.GamingGroupId == currentUser.CurrentGamingGroupId)
                .Select(x => x.Id)
                .FirstOrDefault();

            if (gameDefinitionId != default(int))
            {
                summary.GamingGroupGameDefinitionSummary = _gameDefinitionRetriever.GetGameDefinitionDetails(gameDefinitionId, numberOfRecentlyPlayedGamesToShow);
            }
            return summary;
        }
    }
}