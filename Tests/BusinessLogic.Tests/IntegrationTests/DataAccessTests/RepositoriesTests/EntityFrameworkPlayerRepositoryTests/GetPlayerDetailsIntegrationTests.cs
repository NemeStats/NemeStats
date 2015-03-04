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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerDetailsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (IDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    NemesisHistoryRetriever nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContext);
                    IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    PlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);

                    dbContext.Configuration.LazyLoadingEnabled = false;
                    dbContext.Configuration.ProxyCreationEnabled = false;

                    PlayerDetails playerDetails = playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);
                    Assert.NotNull(playerDetails.PlayerGameResults, "Failed to retrieve PlayerGameResults.");
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayedGames()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (IDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    dbContext.Configuration.LazyLoadingEnabled = false;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    INemesisHistoryRetriever nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContext);
                    IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);
                    PlayerDetails testPlayerDetails = playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);

                    Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame);
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (IDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    dbContext.Configuration.LazyLoadingEnabled = false;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    INemesisHistoryRetriever nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContext);
                    IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);
                    PlayerDetails testPlayerDetails = playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);

                    Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame.GameDefinition);
                }
            }
        }

        [Test]
        public void ItSetsPlayerStatistics()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (IDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    INemesisHistoryRetriever nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContext);
                    IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext,  playerRepository);
                    PlayerDetails playerDetails = playerRetriever.GetPlayerDetails(testPlayer1.Id, 1);

                    Assert.NotNull(playerDetails.PlayerStats);
                }
            }
        }

        [Test]
        public void ItOnlyGetsTheSpecifiedNumberOfRecentGames()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (IDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    int numberOfGamesToRetrieve = 1;

                    INemesisHistoryRetriever nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContext);
                    IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);
                    PlayerDetails playerDetails = playerRetriever.GetPlayerDetails(testPlayer1.Id, numberOfGamesToRetrieve);

                    Assert.AreEqual(numberOfGamesToRetrieve, playerDetails.PlayerGameResults.Count);
                }
            }
        }

        [Test]
        public void ItOrdersPlayerGameResultsByTheDatePlayedDescending()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (IDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    int numberOfGamesToRetrieve = 3;

                    INemesisHistoryRetriever nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContext);
                    IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, playerRepository);
                    PlayerDetails playerDetails = playerRetriever.GetPlayerDetails(testPlayer1.Id, numberOfGamesToRetrieve);

                    long lastTicks = long.MaxValue; ;
                    Assert.IsTrue(playerDetails.PlayerGameResults.Count == numberOfGamesToRetrieve);
                    foreach (PlayerGameResult result in playerDetails.PlayerGameResults)
                    {
                        Assert.GreaterOrEqual(lastTicks, result.PlayedGame.DatePlayed.Ticks);

                        lastTicks = result.PlayedGame.DatePlayed.Ticks;
                    }
                }
            }
        }
    }
}
