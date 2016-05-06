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

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGamesTests
{
    [TestFixture]
    public class GetRecentlyPlayedGameTests : IntegrationTestBase
    {
        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;

                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory,null))
                {
                    PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);

                    List<PlayedGame> playedGames = retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
                    GameDefinition gameDefinition = playedGames[0].GameDefinition;

                    Assert.NotNull(gameDefinition);
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory,null))
                {
                    PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);

                    List<PlayedGame> playedGames = retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
                    ICollection<PlayerGameResult> playerGameResults = playedGames[0].PlayerGameResults;

                    Assert.NotNull(playerGameResults);
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayers()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory,null))
                {
                    PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);

                    List<PlayedGame> playedGames = retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
                    List<Player> players = playedGames[0].PlayerGameResults.Select(
                        playerGameResult => new Player()
                                                {
                                                    Id = playerGameResult.PlayerId,
                                                    Name = playerGameResult.Player.Name,
                                                    Active = playerGameResult.Player.Active
                                                }).ToList();

                    Assert.NotNull(players);
                }
            }
        }

        [Test]
        public void ItReturnsOnlyOneGameIfOneGameIsSpecified()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);
                int one = 1;
                List<PlayedGame> playedGames = retriever.GetRecentGames(one, testUserWithDefaultGamingGroup.CurrentGamingGroupId);

                Assert.AreEqual(one, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsOnlyTwoGamesIfTwoGamesAreSpecified()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);
                int two = 2;
                List<PlayedGame> playedGames = retriever.GetRecentGames(two, testUserWithDefaultGamingGroup.CurrentGamingGroupId);

                Assert.AreEqual(two, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsGamesInDescendingOrderByDatePlayed()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);
                int five = 5;
                List<PlayedGame> playedGames = retriever.GetRecentGames(five, testUserWithDefaultGamingGroup.CurrentGamingGroupId);
                List<PlayedGame> allPlayedGames = dataContext.GetQueryable<PlayedGame>()
                    .Where(game => game.GamingGroupId == testUserWithDefaultGamingGroup.CurrentGamingGroupId)
                    .ToList()
                    .OrderByDescending(playedGame => playedGame.DatePlayed)
                    .ToList();
                for(int i = 0; i<five; i++)
                {
                    Assert.AreEqual(allPlayedGames[i].Id, playedGames[i].Id);
                }
            }
        }

        [Test]
        public void ItReturnsOrderedPlayerRankDescendingWithinAGivenGame()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);
                int five = 5;
                List<PlayedGame> playedGames = retriever.GetRecentGames(five, testUserWithDefaultGamingGroup.CurrentGamingGroupId);

                int lastRank = -1;

                foreach(PlayedGame playedGame in playedGames)
                {
                    foreach(PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
                    {
                        Assert.True(lastRank <= playerGameResult.GameRank);
                        lastRank = playerGameResult.GameRank;
                    }

                    lastRank = -1;
                }
            }
        }

        [Test]
        public void ItOnlyReturnsGamesForTheCurrentUsersGamingGroup()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);
                
                List<PlayedGame> playedGames = retriever.GetRecentGames(20, testUserWithOtherGamingGroup.CurrentGamingGroupId);

                Assert.True(playedGames.All(game => game.GamingGroupId == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }
    }
}
