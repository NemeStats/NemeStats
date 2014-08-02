using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
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

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class EntityFrameworkGamingGroupAccessGranterTestBase
    {
        protected DataContext dataContextMock;
        protected GamingGroupAccessGranter gamingGroupAccessGranter;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 777
            };

            gamingGroupAccessGranter = new EntityFrameworkGamingGroupAccessGranter(dataContextMock);
        }
    }
}
