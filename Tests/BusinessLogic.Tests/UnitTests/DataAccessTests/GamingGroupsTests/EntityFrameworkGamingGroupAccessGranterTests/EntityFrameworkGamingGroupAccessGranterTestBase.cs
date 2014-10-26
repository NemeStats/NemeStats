using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class EntityFrameworkGamingGroupAccessGranterTestBase
    {
        protected IDataContext dataContextMock;
        protected IGamingGroupAccessGranter gamingGroupAccessGranter;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 777
            };

            gamingGroupAccessGranter = new EntityFrameworkGamingGroupAccessGranter(dataContextMock);
        }
    }
}
