using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerIntegrationTests : IntegrationTestBase
    {
        private DataContext dataContext;
        private EntityFrameworkPlayerRepository playerRepository;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dataContext = new NemeStatsDataContext();
        }

        [SetUp]
        public void TestSetUp()
        {
            playerRepository = new EntityFrameworkPlayerRepository(dataContext);
        }

        [Test]
        public void ItReturnsNullIfThePlayerDoesntExist()
        {
            Player player = playerRepository.GetPlayer(-1, testUserWithDefaultGamingGroup);

            Assert.Null(player);
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheRequestedPlayerIsNotInTheCurrentUsersGamingGroup()
        {
            Exception exception = Assert.Throws<UnauthorizedAccessException>(() => playerRepository
                .GetPlayer(testPlayer1.Id, testUserWithOtherGamingGroup));

            string expectedMessage = string.Format(EntityFrameworkPlayerRepository.EXCEPTION_USER_DOES_NOT_HAVE_ACCESS_TO_THIS_PLAYER,
                testUserWithOtherGamingGroup.Id,
                testPlayer1.Id);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ItReturnsTheSpecifiedPlayer()
        {
            Player player = playerRepository.GetPlayer(testPlayer1.Id, testUserWithDefaultGamingGroup);

            Assert.AreEqual(testPlayer1.Id, player.Id);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
