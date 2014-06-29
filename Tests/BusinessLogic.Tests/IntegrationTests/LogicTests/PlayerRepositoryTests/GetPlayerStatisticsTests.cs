using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerStatisticsTests : IntegrationTestBase
    {
        [Test]
        public void ItGetsTheNumberOfTotalGamesPlayed()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayerLogic playerLogic = new PlayerRepository(dbContext);
                PlayerRepository playerRepository = new PlayerRepository(dbContext);
                PlayerStatistics playerStatistics = playerRepository.GetPlayerStatistics(testPlayer1.Id);
                int totalGamesForPlayer1 = testPlayedGames
                        .Count(playedGame => playedGame.PlayerGameResults
                            .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id));
                Assert.AreEqual(totalGamesForPlayer1, playerStatistics.TotalGames);
            }
        }  
    }
}
