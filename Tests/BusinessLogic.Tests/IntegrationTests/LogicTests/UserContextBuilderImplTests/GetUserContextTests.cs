using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models.User;
using NUnit.Framework;

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
    }
}
