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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.Players
{
    [TestFixture]
    public class GetTopPlayersIntegrationTests : IntegrationTestBase
    {
        private PlayerSummaryBuilder playerSummaryBuilderImpl;
        private List<TopPlayer> topPlayersResult;
        private int expectedNumberOfPlayers = 3;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using(IDataContext dataContext = new NemeStatsDataContext())
            {
                playerSummaryBuilderImpl = new PlayerSummaryBuilder(dataContext);

                topPlayersResult = playerSummaryBuilderImpl.GetTopPlayers(3);
            }
        }

        [Test]
        public void ItGetsTheSpecifiedNumberOfPlayers()
        {
            Assert.AreEqual(expectedNumberOfPlayers, topPlayersResult.Count);
        }

        [Test]
        public void ItReturnsThePlayersInDescendingOrderOfTotalNumberOfGamesPlayed()
        {
            int lastNumberofPlayedGames = int.MaxValue;

            foreach(TopPlayer topPlayer in topPlayersResult)
            {
                Assert.LessOrEqual(topPlayer.TotalNumberOfGamesPlayed, lastNumberofPlayedGames);
                lastNumberofPlayedGames = topPlayer.TotalNumberOfGamesPlayed;
            }
        }

        [Test]
        public void ItHasAllOfTheData()
        {
            TopPlayer firstResult = topPlayersResult[0];
            Assert.Greater(firstResult.TotalNumberOfGamesPlayed, 0);
            Assert.Greater(firstResult.TotalPoints, 0);
            Assert.True(!string.IsNullOrWhiteSpace(firstResult.PlayerName));
            Assert.Greater(firstResult.PlayerId, 0);
        }

        [Test]
        public void ItCalculatesTheWinPercentageCorrectly()
        {
            int totalGamesPlayed = 100;
            int totalGamesWon = 20;

            int actualWinPercentage = playerSummaryBuilderImpl.CalculateWinPercentage(totalGamesWon, totalGamesPlayed);

            Assert.AreEqual(20, actualWinPercentage);
        }
    }
}
