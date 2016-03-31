using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Points
{
    public class GlobalPointsRecalculator
    {
        public void RecalculatePoints(IDataContext dataContext, IPointsCalculator pointsCalculator, List<PlayedGameToRecalculate> playedGamesToRecalculate)
        {
            int counter = 0;
            foreach (var playedGame in playedGamesToRecalculate)
            {
                var playerRanks = playedGame.PlayerGameResults.Select(x => new PlayerRank
                {
                    PlayerId = x.PlayerId,
                    GameRank = x.GameRank
                }).ToList();

                var newPoints = pointsCalculator.CalculatePoints(playerRanks, playedGame.BoardGameGeekGameDefinition);

                var applicationUserForThisGamingGroup = new ApplicationUser()
                {
                    CurrentGamingGroupId = playedGame.GamingGroupId
                };

                foreach (var playerGameResult in playedGame.PlayerGameResults)
                {
                    var scorecard = newPoints[playerGameResult.PlayerId];
                    playerGameResult.NemeStatsPointsAwarded = scorecard.BasePoints;
                    playerGameResult.GameDurationBonusPoints = scorecard.GameDurationBonusPoints;
                    playerGameResult.GameWeightBonusPoints = scorecard.GameWeightBonusPoints;
                    dataContext.Save(playerGameResult, applicationUserForThisGamingGroup);
                }

                Debug.WriteLine("{0} games updated... last PlayedGame.Id is {1}", ++counter, playedGame.PlayedGameId);
            }
        }

        public void RecalculateAllPoints(IDataContext dataContext, IPointsCalculator pointsCalculator, int startPlayedGameId, int endPlayedGameId = int.MaxValue)
        {
            var allPlayedGames = (from PlayedGame playedGame in dataContext.GetQueryable<PlayedGame>()
                                  where playedGame.Id >= startPlayedGameId && playedGame.Id < endPlayedGameId
                                  select new PlayedGameToRecalculate
                                  {
                                      PlayedGameId = playedGame.Id,
                                      GamingGroupId = playedGame.GamingGroupId,
                                      PlayerGameResults = playedGame.PlayerGameResults,
                                      BoardGameGeekGameDefinition = playedGame.GameDefinition.BoardGameGeekGameDefinition
                                  }).OrderBy(x => x.PlayedGameId).ToList();

            RecalculatePoints(dataContext, pointsCalculator, allPlayedGames);
        }

        public void RecalculateAllPointsForGamesWithNoPlayTime(IDataContext dataContext, IPointsCalculator pointsCalculator)
        {
            var allPlayedGames = (from PlayedGame playedGame in dataContext.GetQueryable<PlayedGame>()
                                  where (playedGame.GameDefinition.BoardGameGeekGameDefinition == null 
                                  || (playedGame.GameDefinition.BoardGameGeekGameDefinition.MinPlayTime == null
                                        && playedGame.GameDefinition.BoardGameGeekGameDefinition.MaxPlayTime == null))
                                  select new PlayedGameToRecalculate
                                  {
                                      PlayedGameId = playedGame.Id,
                                      GamingGroupId = playedGame.GamingGroupId,
                                      PlayerGameResults = playedGame.PlayerGameResults,
                                      BoardGameGeekGameDefinition = null
                                  }).OrderBy(x => x.PlayedGameId).ToList();

            RecalculatePoints(dataContext, pointsCalculator, allPlayedGames);
        }

        public class PlayedGameToRecalculate
        {
            public int PlayedGameId { get; set; }
            public int GamingGroupId { get; set; }
            public IList<PlayerGameResult> PlayerGameResults { get; set; }
            public BoardGameGeekGameDefinition BoardGameGeekGameDefinition { get; set; }
        }
    }
}
