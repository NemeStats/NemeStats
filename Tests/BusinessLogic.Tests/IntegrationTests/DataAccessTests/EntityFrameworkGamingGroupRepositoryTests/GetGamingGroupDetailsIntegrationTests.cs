using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGamingGroupRepositoryTests
{
    [TestFixture]
    public class GetGamingGroupDetailsIntegrationTests : EntityFrameworkGamingGroupRepositoryTestBase
    {
        //TODO add the many to many relationship so we can get applicationUsers
        /*
        [Test]
        public void ItEagerlyLoadsTheApplicationUsers()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                EntityFrameworkGamingGroupRepository repository = new EntityFrameworkGamingGroupRepository(dbContext);
                GamingGroup gamingGroup = repository.GetGamingGroupDetails(testGamingGroup.Id, testUserContextForUserWithDefaultGamingGroup);

                Assert.Greater(gamingGroup.Users.Count(), 0);
            }
        }*/

        [Test]
        public void ItEagerlyLoadsTheOwningUser()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                EntityFrameworkGamingGroupRepository repository = new EntityFrameworkGamingGroupRepository(dbContext);
                GamingGroup gamingGroup = repository.GetGamingGroupDetails(testGamingGroup.Id, testUserContextForUserWithDefaultGamingGroup);

                Assert.AreEqual(testUserContextForUserWithDefaultGamingGroup.ApplicationUserId, gamingGroup.OwningUser.Id);
            }
        }

        [Test]
        public void ItEagerlyLoadsGamingGroupInvitations()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                EntityFrameworkGamingGroupRepository repository = new EntityFrameworkGamingGroupRepository(dbContext);
                GamingGroup gamingGroup = repository.GetGamingGroupDetails(testGamingGroup.Id, testUserContextForUserWithDefaultGamingGroup);

                Assert.AreSame(gamingGroup.GamingGroupInvitations, gamingGroup.GamingGroupInvitations);
            }
        }
    }
}
