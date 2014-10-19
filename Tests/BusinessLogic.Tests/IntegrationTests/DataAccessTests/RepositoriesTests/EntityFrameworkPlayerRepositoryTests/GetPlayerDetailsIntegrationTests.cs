using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
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
                    PlayerRetriever playerRetriever = new PlayerRetriever(dataContext, nemesisHistoryRetriever);

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
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, nemesisHistoryRetriever);
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
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, nemesisHistoryRetriever);
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
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, nemesisHistoryRetriever);
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
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, nemesisHistoryRetriever);
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
                    IPlayerRetriever playerRetriever = new PlayerRetriever(dataContext, nemesisHistoryRetriever);
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
