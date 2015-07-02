using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.Players
{
    [TestFixture]
    public class GetPlayerStatisticsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItReturnsZeroesWhenThereAreNoPlayedGames()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                var playerRetriever = new PlayerRetriever(dataContext, new EntityFrameworkPlayerRepository(dataContext));

                var statistics = playerRetriever.GetPlayerStatistics(testPlayerWithNoPlayedGames.Id);

                Assert.That(statistics.TotalGames, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesLost, Is.EqualTo(0));
                Assert.That(statistics.TotalGames, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesWon, Is.EqualTo(0));
                Assert.That(statistics.TotalPoints, Is.EqualTo(0));
                Assert.That(statistics.WinPercentage, Is.EqualTo(0));

            } 
        }
    }
}
