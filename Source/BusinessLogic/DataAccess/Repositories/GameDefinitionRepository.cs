using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace BusinessLogic.DataAccess.Repositories
{
    public class GameDefinitionRepository : IGameDefinitionRepository
    {
        private readonly IDataContext _dataContext;

        public GameDefinitionRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private const string TRENDING_GAMES_SQL = @"SELECT TOP {0} COUNT(DISTINCT GameDefinition.GamingGroupId) AS GamingGroupsPlayingThisGame, 
                BoardGameGeekGameDefinition.Id As BoardGameGeekGameDefinitionId, BoardGameGeekGameDefinition.Name, 
                BoardGameGeekGameDefinition.Thumbnail AS ThumbnailImageUrl
                FROM GameDefinition 
                INNER JOIN BoardGameGeekGameDefinition ON BoardGameGeekGameDefinition.Id = GameDefinition.BoardGameGeekGameDefinitionId AND GameDefinition.Active = 1
                WHERE EXISTS (SELECT TOP 1 1 FROM PlayedGame WHERE PlayedGame.GameDefinitionId = GameDefinition.Id AND PlayedGame.DatePlayed >= @startDate)
                GROUP BY BoardGameGeekGameDefinition.Id, BoardGameGeekGameDefinition.Name, BoardGameGeekGameDefinition.Thumbnail
                ORDER BY GamingGroupsPlayingThisGame DESC;";

        public IList<TrendingGame> GetTrendingGames(int maxNumberOfGames, int numberOfDaysOfTrendingGames)
        {
            //--this particular query is expensive and times out a lot. Until we can come up with a better query just increase the timeout.
            _dataContext.SetCommandTimeout(45);
            var startDate = DateTime.Now.Date.AddDays(-1 * numberOfDaysOfTrendingGames);

            var formattedSql = string.Format(TRENDING_GAMES_SQL, maxNumberOfGames);
            var dbRawSqlQuery = _dataContext.MakeRawSqlQuery<TrendingGame>(formattedSql,
                new SqlParameter("startDate", startDate));

            var trendingGames = dbRawSqlQuery.ToList();

            var boardGameGeekGameDefinitionIds = trendingGames.Select(x => x.BoardGameGeekGameDefinitionId).ToList();
            var numberOfGamesPlayedDictionary = _dataContext.GetQueryable<PlayedGame>()
                .Where(x => x.GameDefinition.Active 
                            && x.DatePlayed >= startDate
                            && boardGameGeekGameDefinitionIds.Contains((int)x.GameDefinition.BoardGameGeekGameDefinitionId))
                .GroupBy(x => x.GameDefinition.BoardGameGeekGameDefinitionId)
                .Select(x => new
                {
                    BoardGameGeekGameDefinitionId = x.Key,
                    Count = x.Count()
                }).ToDictionary(y => y.BoardGameGeekGameDefinitionId, z => z.Count);


            trendingGames.ForEach(x => x.GamesPlayed = numberOfGamesPlayedDictionary[x.BoardGameGeekGameDefinitionId]);

            return trendingGames;
        }
    }
}
