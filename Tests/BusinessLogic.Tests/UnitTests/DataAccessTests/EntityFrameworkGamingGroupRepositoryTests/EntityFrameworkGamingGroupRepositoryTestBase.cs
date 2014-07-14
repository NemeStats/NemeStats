using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkGamingGroupRepositoryTests
{
    [TestFixture]
    public class EntityFrameworkGamingGroupRepositoryTestBase
    {
        protected EntityFrameworkGamingGroupRepository gamingGroupRepositoryPartialMock;
        protected NemeStatsDbContext dbContextMock;
        protected UserContext userContext;
        protected GamingGroup gamingGroup;

        [SetUp]
        public void SetUp()
        {
            gamingGroup = new GamingGroup()
            {
                Name = "gaming group name"
            };
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 999
            };
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();

            gamingGroupRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkGamingGroupRepository>(dbContextMock);
        }
    }
}
