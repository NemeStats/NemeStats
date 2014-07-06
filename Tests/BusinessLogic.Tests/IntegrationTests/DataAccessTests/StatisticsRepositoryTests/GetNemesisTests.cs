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

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.StatisticsRepositoryTests
{
    [TestFixture]
    public class GetNemesisTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private PlayerLogic playerLogic;
        private UserContextBuilder userContextBuilder;

        [SetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
            userContextBuilder = new UserContextBuilderImpl();
            playerLogic = new PlayerRepository(dbContext, userContextBuilder);
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentageAgainstMe()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id);

            Assert.AreEqual(testPlayer4.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ANemesisMustBeActive()
        {
            //player 5 is inactive but beat player 1 three times
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id);

            Assert.AreNotEqual(testPlayer5.Id, nemesis.NemesisPlayerId);
        }
        
        [Test]
        public void ItReturnsANullNemesisIfThereIsNoNemesis()
        {
            //player 5 is inactive but beat player 1 three times
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer5.Id);

            Assert.True(nemesis is NullNemesis);
        }

        [Test]
        public void ANemesisMustHaveWonAtLeastACertainNumberOfGames()
        {
            //player2 beat player5 once (100% of the time) but this isn't enough to be a nemesis
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer5.Id);

            Assert.AreNotEqual(testPlayer2.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerId()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id);

            Assert.AreEqual(testPlayer4.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerName()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id);

            Assert.AreEqual(testPlayer4.Name, nemesis.NemesisPlayerName);
        }

        [Test]
        public void ItSetsTheLossPercentageVersusTheNemesis()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id);

            Assert.AreEqual(100, nemesis.LossPercentageVersusNemesis);
        }

        [Test]
        public void ItSetsTheNumberOfGamesLostVersusTheNemesis()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id);

            Assert.AreEqual(3, nemesis.GamesLostVersusNemesis);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
