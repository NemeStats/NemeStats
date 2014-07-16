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

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.StatisticsRepositoryTests
{
    [TestFixture]
    public class GetNemesisTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private PlayerRepository playerLogic;
        private UserContextBuilder userContextBuilder;

        [SetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
            userContextBuilder = new UserContextBuilderImpl();
            playerLogic = new EntityFrameworkPlayerRepository(dbContext);
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentageAgainstMe()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer4.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ANemesisMustBeActive()
        {
            //player 5 is inactive but beat player 1 three times
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreNotEqual(testPlayer5.Id, nemesis.NemesisPlayerId);
        }
        
        [Test]
        public void ItReturnsANullNemesisIfThereIsNoNemesis()
        {
            //player 5 is inactive but beat player 1 three times
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer5.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.True(nemesis is NullNemesis);
        }

        [Test]
        public void ANemesisMustHaveWonAtLeastACertainNumberOfGames()
        {
            //player2 beat player5 once (100% of the time) but this isn't enough to be a nemesis
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer5.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreNotEqual(testPlayer2.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerId()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer4.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerName()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer4.Name, nemesis.NemesisPlayerName);
        }

        [Test]
        public void ItSetsTheLossPercentageVersusTheNemesis()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreEqual(100, nemesis.LossPercentageVersusNemesis);
        }

        [Test]
        public void ItSetsTheNumberOfGamesLostVersusTheNemesis()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserContextForUserWithDefaultGamingGroup);

            Assert.AreEqual(3, nemesis.GamesLostVersusNemesis);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
