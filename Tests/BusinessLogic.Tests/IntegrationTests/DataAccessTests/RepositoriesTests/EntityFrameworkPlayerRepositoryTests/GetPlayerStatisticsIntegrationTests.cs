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
            using (DataContext dataContext = new NemeStatsDataContext())
            {
                PlayerRepository playerLogic = new EntityFrameworkPlayerRepository(dataContext);
                PlayerStatistics playerStatistics = playerLogic.GetPlayerStatistics(testPlayer1.Id, testUserWithDefaultGamingGroup);
                int totalGamesForPlayer1 = testPlayedGames
                        .Count(playedGame => playedGame.PlayerGameResults
                            .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id));
                Assert.AreEqual(totalGamesForPlayer1, playerStatistics.TotalGames);
            }
        }

        [Test]
        public void ItOnlyIncludesStatisticsForPlayersWhoShareTheSameGamingGroupAsTheCurrentUser()
        {
            using (DataContext dataContext = new NemeStatsDataContext())
            {
                PlayerRepository playerLogic = new EntityFrameworkPlayerRepository(dataContext);
                PlayerStatistics playerStatistics = playerLogic.GetPlayerStatistics(testPlayer1.Id, testUserWithOtherGamingGroup);
                
                Assert.AreEqual(0, playerStatistics.TotalGames);
            }
        } 
    }
}
