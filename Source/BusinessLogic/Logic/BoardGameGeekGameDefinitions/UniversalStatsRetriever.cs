using System.Data.Entity;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class UniversalStatsRetriever : Cacheable<int, UniversalGameStats>, IUniversalStatsRetriever
    {
        private readonly IDataContext _dataContext;

        public UniversalStatsRetriever(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override UniversalGameStats GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var definition = _dataContext
                .GetQueryable<BoardGameGeekGameDefinition>()
                .Where(x => x.Id == boardGameGeekGameDefinitionId)
                .Include(x => x.GameDefinitions)
                .Include(x => x.GameDefinitions.Select(y => y.PlayedGames))
                .SingleOrDefault();

            if (definition == null)
            {
                throw new EntityDoesNotExistException<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId);
            }

            if (definition.GameDefinitions.Any())
            {
                return new UniversalGameStats
                {
                    AveragePlayersPerGame = definition.GameDefinitions.Where(gd => gd.PlayedGames.Any()).Average(y => y.PlayedGames.Select(p => p.NumberOfPlayers).Average()),
                    TotalNumberOfGamesPlayed = definition.GameDefinitions.Sum(y => y.PlayedGames.Count),
                    TotalGamingGroupsWithThisGame = definition.GameDefinitions.Count(g => g.Active && g.PlayedGames.Any())
                };
            }
            else
            {
                return new UniversalGameStats();
            }
        }
    }
}