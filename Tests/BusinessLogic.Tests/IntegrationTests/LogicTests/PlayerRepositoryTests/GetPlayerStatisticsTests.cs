using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Statistics;
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
        private PlayerRepository playerRepository;

        [SetUp]
        public void SetUp()
        {
            playerRepository = new PlayerRepository(dbContext);
        }

        [Test]
        public void ItGetsTheNumberOfTotalGamesPlayed()
        {
            PlayerStatistics playerStatistics = playerRepository.GetPlayerStatistics(testPlayer1.Id);
            int totalGamesForPlayer1 = testPlayedGames
                    .Count(playedGame => playedGame.PlayerGameResults
                        .Any(playerGameResult => playerGameResult.PlayerId == testPlayer1.Id));
            Assert.AreEqual(totalGamesForPlayer1, playerStatistics.TotalGames);
        }  
    }
}
