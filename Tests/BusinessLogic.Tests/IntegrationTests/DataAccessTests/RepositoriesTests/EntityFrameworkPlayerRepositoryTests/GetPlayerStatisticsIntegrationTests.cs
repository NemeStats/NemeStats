using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerStatisticsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItGetsTheNumberOfTotalGamesPlayed()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayerRepository playerLogic = new EntityFrameworkPlayerRepository(dataContext);
                PlayerStatistics playerStatistics = playerLogic.GetPlayerStatistics(testPlayer1.Id);
                int totalGamesForPlayer1 = testPlayedGames
                        .Count(playedGame => playedGame.PlayerGameResults
                            .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id));
                Assert.AreEqual(totalGamesForPlayer1, playerStatistics.TotalGames);
            }
        }

        [Test]
        public void ItGetsTheTotalPoints()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                IPlayerRepository playerLogic = new EntityFrameworkPlayerRepository(dataContext);
                PlayerStatistics playerStatistics = playerLogic.GetPlayerStatistics(testPlayer1.Id);

                int totalPoints = 0;

                foreach(PlayedGame playedGame in testPlayedGames)
                {
                    if(playedGame.PlayerGameResults.Any(result => result.PlayerId == testPlayer1.Id))
                    {
                        totalPoints += playedGame.PlayerGameResults.Where(result => result.PlayerId == testPlayer1.Id).First().GordonPoints;
                    }
                }

                Assert.AreEqual(totalPoints, playerStatistics.TotalPoints);
            }
        }
    }
}
