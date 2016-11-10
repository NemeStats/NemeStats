using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.UniversalGameDefinitions
{
    public class UniversalGameRetriever : Cacheable<int, UniversalGameData>, IUniversalGameRetriever
    {
        private readonly IDataContext _dataContext;

        public UniversalGameRetriever(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override UniversalGameData GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var result = _dataContext.GetQueryable<BoardGameGeekGameDefinition>().Where(x => x.Id == boardGameGeekGameDefinitionId)
                .Select(x => new UniversalGameData
                {
                    BoardGameGeekGameDefinitionId = x.Id,
                    BoardGameGeekAverageWeight = x.AverageWeight,
                    BoardGameGeekDescription = x.Description,
                    BoardGameGeekYearPublished = x.YearPublished,
                    ImageUrl = x.Image,
                    MaxPlayers = x.MaxPlayers,
                    MinPlayers = x.MinPlayers,
                    MinPlayTime = x.MinPlayTime,
                    MaxPlayTime = x.MaxPlayTime,
                    Name = x.Name,
                    ThumbnailImageUrl = x.Thumbnail,
                    BoardGameGeekMechanics = x.Mechanics.Select(y => y.MechanicName).ToList(),
                    BoardGameGeekCategories = x.Categories.Select(y => y.CategoryName).ToList(),
                    AveragePlayersPerGame = x.GameDefinitions.Average(y => y.PlayedGames.Select(p => p.NumberOfPlayers).Average()),
                    TotalNumberOfGamesPlayed = x.GameDefinitions.Sum(y => y.PlayedGames.Count),
                    TotalGamingGroupsWithThisGame = x.GameDefinitions.GroupBy(y => y.GamingGroupId).Select(z => z.Key).Count()
                }).FirstOrDefault();

            if (result == null)
            {
                throw new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), boardGameGeekGameDefinitionId);
            }

            return result;
        }
    }
}