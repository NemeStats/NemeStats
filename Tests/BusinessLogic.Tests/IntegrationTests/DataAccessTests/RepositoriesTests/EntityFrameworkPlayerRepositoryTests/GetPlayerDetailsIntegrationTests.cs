#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerDetailsIntegrationTests : IntegrationTestBase
    {
        private PlayerRetriever _playerRetriever;

        [SetUp]
        public void SetUp()
        {
            _playerRetriever = GetInstance<PlayerRetriever>();
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            var playerDetails = _playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);
            Assert.NotNull(playerDetails.PlayerGameResults, "Failed to retrieve PlayerGameResults.");
        }

        [Test]
        public void ItEagerlyFetchesPlayedGames()
        {
            var testPlayerDetails = _playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);

            Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame);
        }

        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            var testPlayerDetails = _playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);

            Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame.GameDefinition);
        }

        [Test]
        public void ItSetsPlayerStatistics()
        {
            var playerDetails = _playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);

            Assert.NotNull(playerDetails.PlayerStats);
        }

        [Test]
        public void ItOnlyGetsTheSpecifiedNumberOfRecentGames()
        {
            int numberOfGamesToRetrieve = 2;
            var playerDetails = _playerRetriever.GetPlayerDetails(testPlayer1.Id, numberOfGamesToRetrieve);

            Assert.AreEqual(numberOfGamesToRetrieve, playerDetails.PlayerGameResults.Count);
        }

        [Test]
        public void ItOrdersPlayerGameResultsByTheDatePlayedDescending()
        {
            var numberOfGamesToRetrieve = 3;
            var playerDetails = _playerRetriever.GetPlayerDetails(testPlayer1.Id, numberOfGamesToRetrieve);

            var lastTicks = long.MaxValue; ;
            Assert.IsTrue(playerDetails.PlayerGameResults.Count == numberOfGamesToRetrieve);
            foreach (var result in playerDetails.PlayerGameResults)
            {
                Assert.GreaterOrEqual(lastTicks, result.PlayedGame.DatePlayed.Ticks);

                lastTicks = result.PlayedGame.DatePlayed.Ticks;
            }
        }
    }
}
