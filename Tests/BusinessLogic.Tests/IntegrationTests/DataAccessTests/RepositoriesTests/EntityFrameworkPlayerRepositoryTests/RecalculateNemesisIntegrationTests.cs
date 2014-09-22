using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Tests.IntegrationTests.LogicTests;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using Rhino.Mocks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using BusinessLogic.Logic;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Users;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetNemesisTests : IntegrationTestBase
    {
        private IDataContext dataContext;
        private IPlayerRepository playerLogic;

        [SetUp]
        public void SetUp()
        {
            dataContext = new NemeStatsDataContext();
            playerLogic = new EntityFrameworkPlayerRepository(dataContext);
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentageAgainstMe()
        {
            //player 4 beat player 1 3 times
            Assert.AreEqual(testPlayer4.Id, testPlayer1.Nemesis.NemesisPlayerId);
        }

        [Test]
        public void ANemesisMustBeActive()
        {
            //player 5 is inactive but beat player 1 three times
            Assert.AreNotEqual(testPlayer5.Id, testPlayer1.Nemesis.NemesisPlayerId);
        }
        
        [Test]
        public void ItReturnsANullNemesisIfThereIsNoNemesis()
        {
            //player 5 has no nemesis
            Assert.True(testPlayer5.Nemesis is NullNemesis);
        }

        [Test]
        public void ANemesisMustHaveWonAtLeastACertainNumberOfGames()
        {
            //player2 beat player5 once (100% of the time) but this isn't enough to be a nemesis
            Assert.AreNotEqual(testPlayer2.Id, testPlayer5.Nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerId()
        {
            Assert.AreEqual(testPlayer4.Id, testPlayer1.Nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheLossPercentageVersusTheNemesis()
        {
            //player 1 lost 100% of their games against player 4
            Assert.AreEqual(100, testPlayer1.Nemesis.LossPercentage);
        }

        [Test]
        public void ItSetsTheNumberOfGamesLostVersusTheNemesis()
        {
            Assert.AreEqual(3, testPlayer1.Nemesis.NumberOfGamesLost);
        }

        [Test]
        public void ItSetsTheNemesisIdOnThePlayer()
        {
            using(NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                using(NemeStatsDataContext nemeStatsDataContext = new NemeStatsDataContext())
                {
                    Player player1 = nemeStatsDataContext.FindById<Player>(testPlayer1.Id);

                    Assert.NotNull(player1.NemesisId);
                }
            }
        }

        [Test]
        public void ItClearsTheNemesisIdIfThePlayerHasNoNemesis()
        {
            using (NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext nemeStatsDataContext = new NemeStatsDataContext())
                {
                    Player player5 = nemeStatsDataContext.FindById<Player>(testPlayer5.Id);

                    Assert.Null(player5.NemesisId);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
