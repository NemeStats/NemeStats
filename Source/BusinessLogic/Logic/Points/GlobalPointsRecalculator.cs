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
        public void RecalculateAllPoints(IDataContext dataContext, IPointsCalculator pointsCalculator, int startPlayedGameId, int endPlayedGameId)
        {
            var allPlayedGames = (from PlayedGame playedGame in dataContext.GetQueryable<PlayedGame>()
                                  where playedGame.Id >= startPlayedGameId && playedGame.Id < endPlayedGameId
                                  select new
                                  {
                                      playedGame.Id,
                                      playedGame.GamingGroupId,
                                      playedGame.PlayerGameResults,
                                      playedGame.GameDefinition.BoardGameGeekGameDefinition
                                  }).OrderBy(x => x.Id).ToList();

            int counter = 0;
            foreach (var playedGame in allPlayedGames)
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
                dataContext.CommitAllChanges();
                if (counter % 100 == 0)
                {
                    Debug.WriteLine("{0} games updated...", counter);
                }
            }
        }
    }
}
