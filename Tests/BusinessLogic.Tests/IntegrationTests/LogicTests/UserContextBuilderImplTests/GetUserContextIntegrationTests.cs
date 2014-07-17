using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.UserContextBuilderImplTests
{
    [TestFixture]
    public class GetUserContextIntegrationTests : IntegrationTestBase
    {
        private UserContextBuilderImpl contextBuilder;
        private UserContext userContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                contextBuilder = new UserContextBuilderImpl();
                userContext = contextBuilder.GetUserContext(testUserContextForUserWithDefaultGamingGroup.ApplicationUserId, dbContext);
            }
        }

        [Test]
        public void ItSetsTheUserId()
        {
            Assert.AreEqual(testUserContextForUserWithDefaultGamingGroup.ApplicationUserId, userContext.ApplicationUserId);
        }

        [Test]
        public void ItSetsTheCurrentGamingGroupId()
        {
            Assert.AreEqual(testGamingGroup.Id, userContext.GamingGroupId);
        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfTheUserDoesntExist()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                string userIdThatDoesntExist = "user id that doesnt exist";
                UserContextBuilderImpl contextBuilder = new UserContextBuilderImpl();
                Exception exception = Assert.Throws<KeyNotFoundException>(
                    () => contextBuilder.GetUserContext(userIdThatDoesntExist, dbContext));

                string message = string.Format(
                    UserContextBuilderImpl.EXCEPTION_MESSAGE_USER_NOT_FOUND,
                    userIdThatDoesntExist);
                Assert.AreEqual(message, exception.Message);
            }
        }
    }
}
