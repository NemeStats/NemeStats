using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Points
{
    public class GlobalPointsRecalculator
    {
        public void RecalculateAllPoints(IDataContext dataContext, int startPlayedGameId, int endPlayedGameId)
        {
            var allPlayedGames = (from PlayedGame playedGame in dataContext.GetQueryable<PlayedGame>()
                                  where playedGame.Id >= startPlayedGameId && playedGame.Id < endPlayedGameId
                                  select new
                                  {
                                      playedGame.Id,
                                      playedGame.GamingGroupId,
                                      playedGame.PlayerGameResults
                                  }).ToList();

            foreach (var playedGame in allPlayedGames)
            {
                var playerRanks = playedGame.PlayerGameResults.Select(x => new PlayerRank
                {
                    PlayerId = x.PlayerId,
                    GameRank = x.GameRank
                }).ToList();

                var newPoints = PointsCalculator.CalculatePoints(playerRanks);
                
                ApplicationUser applicationUserForThisGamingGroup = new ApplicationUser()
                {
                    CurrentGamingGroupId = playedGame.GamingGroupId
                };

                foreach (PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
                {
                    playerGameResult.NemeStatsPointsAwarded = newPoints[playerGameResult.PlayerId];
                    dataContext.Save(playerGameResult, applicationUserForThisGamingGroup);
                }
            }
        }
    }
}
