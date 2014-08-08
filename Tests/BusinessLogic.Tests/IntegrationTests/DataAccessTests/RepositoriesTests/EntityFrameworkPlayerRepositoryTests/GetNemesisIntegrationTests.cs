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
        private DataContext dataContext;
        private PlayerRepository playerLogic;

        [SetUp]
        public void SetUp()
        {
            dataContext = new NemeStatsDataContext();
            playerLogic = new EntityFrameworkPlayerRepository(dataContext);
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentageAgainstMe()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer4.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ANemesisMustBeActive()
        {
            //player 5 is inactive but beat player 1 three times
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreNotEqual(testPlayer5.Id, nemesis.NemesisPlayerId);
        }
        
        [Test]
        public void ItReturnsANullNemesisIfThereIsNoNemesis()
        {
            //player 5 is inactive but beat player 1 three times
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer5.Id, testUserWithDefaultGamingGroup);

            Assert.True(nemesis is NullNemesis);
        }

        [Test]
        public void ANemesisMustHaveWonAtLeastACertainNumberOfGames()
        {
            //player2 beat player5 once (100% of the time) but this isn't enough to be a nemesis
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer5.Id, testUserWithDefaultGamingGroup);

            Assert.AreNotEqual(testPlayer2.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerId()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer4.Id, nemesis.NemesisPlayerId);
        }

        [Test]
        public void ItSetsTheNemesisPlayerName()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer4.Name, nemesis.NemesisPlayerName);
        }

        [Test]
        public void ItSetsTheLossPercentageVersusTheNemesis()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreEqual(100, nemesis.LossPercentageVersusNemesis);
        }

        [Test]
        public void ItSetsTheNumberOfGamesLostVersusTheNemesis()
        {
            Nemesis nemesis = playerLogic.GetNemesis(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreEqual(3, nemesis.GamesLostVersusNemesis);
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
