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
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Utility;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGamesTests
{
    [TestFixture]
    public class GetRecentlyPlayedGameTests : IntegrationTestBase
    {
        private PlayedGameRetriever _retriever;

        [SetUp]
        public void SetUp()
        {
            _retriever = GetInstance<PlayedGameRetriever>();
        }

        [Test]
        public void ItFetchesGameDefinitions()
        {
            var playedGames = _retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
            var gameDefinition = playedGames[0].GameDefinition;

            Assert.NotNull(gameDefinition);
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            var playedGames = _retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
            ICollection<PlayerGameResult> playerGameResults = playedGames[0].PlayerGameResults;

            Assert.NotNull(playerGameResults);
        }

        [Test]
        public void ItEagerlyFetchesPlayers()
        {
            var playedGames = _retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
            var players = playedGames[0].PlayerGameResults.Select(
                playerGameResult => new Player()
                                        {
                                            Id = playerGameResult.PlayerId,
                                            Name = playerGameResult.Player.Name,
                                            Active = playerGameResult.Player.Active
                                        }).ToList();

            Assert.NotNull(players);
        }

        [Test]
        public void ItReturnsOnlyOneGameIfOneGameIsSpecified()
        {
            var one = 1;
            var playedGames = _retriever.GetRecentGames(one, testUserWithDefaultGamingGroup.CurrentGamingGroupId);

            Assert.AreEqual(one, playedGames.Count());
        }

        [Test]
        public void ItReturnsOnlyTwoGamesIfTwoGamesAreSpecified()
        {
            var two = 2;
            var playedGames = _retriever.GetRecentGames(two, testUserWithDefaultGamingGroup.CurrentGamingGroupId);

            Assert.AreEqual(two, playedGames.Count());
        }

        [Test]
        public void ItReturnsGamesInDescendingOrderByDatePlayed()
        {
            using (var dataContext = new NemeStatsDataContext())
            {
                var dateRangeFilter = new BasicDateRangeFilter();
                var five = 5;
                List<PlayedGame> playedGames = _retriever.GetRecentGames(five, testUserWithDefaultGamingGroup.CurrentGamingGroupId, dateRangeFilter);
                var allPlayedGames = dataContext.GetQueryable<PlayedGame>()
                    .Where(game => game.GamingGroupId == testUserWithDefaultGamingGroup.CurrentGamingGroupId
                        && game.DatePlayed <= dateRangeFilter.ToDate)
                    .ToList()
                    .OrderByDescending(playedGame => playedGame.DatePlayed)
                    .ToList();
                for(var i = 0; i<five; i++)
                {
                    Assert.AreEqual(allPlayedGames[i].Id, playedGames[i].Id);
                }
            }
        }

        [Test]
        public void ItReturnsOrderedPlayerRankDescendingWithinAGivenGame()
        {
            var five = 5;
            var playedGames = _retriever.GetRecentGames(five, testUserWithDefaultGamingGroup.CurrentGamingGroupId);

            var lastRank = -1;

            foreach(var playedGame in playedGames)
            {
                foreach(var playerGameResult in playedGame.PlayerGameResults)
                {
                    Assert.True(lastRank <= playerGameResult.GameRank);
                    lastRank = playerGameResult.GameRank;
                }

                lastRank = -1;
            }
        }

        [Test]
        public void ItOnlyReturnsGamesForTheCurrentUsersGamingGroup()
        {
            var playedGames = _retriever.GetRecentGames(20, testUserWithOtherGamingGroup.CurrentGamingGroupId);

            Assert.True(playedGames.All(game => game.GamingGroupId == testUserWithOtherGamingGroup.CurrentGamingGroupId));
        }
    }
}
