using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Exceptions;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.UserContextBuilderImplTests
{
    [TestFixture]
    public class GetUserContextTests : IntegrationTestBase
    {
        private UserContextBuilderImpl contextBuilder;
        private UserContext userContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            base.FixtureSetUp();

            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                contextBuilder = new UserContextBuilderImpl();
                userContext = contextBuilder.GetUserContext(testApplicationUserNameForUserWithDefaultGamingGroup, dbContext);
            }
        }

        [Test]
        public void ItSetsTheUserId()
        {
            Assert.AreEqual(testApplicationUserWithDefaultGamingGroup.Id, userContext.ApplicationUserId);
        }

        [Test]
        public void ItSetsTheCurrentGamingGroupId()
        {
            Assert.AreEqual(gamingGroup.Id, userContext.GamingGroupId);
        }

        [Test]
        public void ItThrowsANoCurrentGamingGroupExceptionIfTheUserDoesntHaveAGamingGroupSet()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                contextBuilder = new UserContextBuilderImpl();
                Exception exception = Assert.Throws<NoCurrentGamingGroupException>(
                    () => contextBuilder.GetUserContext(testApplicationUserNameForUserWithoutDefaultGamingGroup, dbContext));

                string[] expectedExceptionParameters = new string[]
                {
                    testApplicationUserWithoutDefaultGamingGroup.Id,
                    testApplicationUserWithoutDefaultGamingGroup.UserName
                };
                Assert.AreEqual(string.Format(UserContextBuilderImpl.EXCEPTION_MESSAGE_NO_CURRENT_GAMING_GROUP, expectedExceptionParameters), exception.Message);
            }
        }
    }
}
