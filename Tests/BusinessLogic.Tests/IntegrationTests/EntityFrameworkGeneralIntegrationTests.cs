using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using System.Data.Entity.Migrations;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    public class EntityFrameworkGeneralIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void TheAddOrInsertExtensionMethodSetsTheIdOnNewEntities()
        {
            using(NemeStatsDataContext dataContext = new NemeStatsDataContext(
                new NemeStatsDbContext(), 
                new SecuredEntityValidatorFactory()))
            {
                GamingGroup gamingGroup = new GamingGroup()
                {
                    Name = "new gaming group without an ID yet",
                    OwningUserId = testUserWithDefaultGamingGroup.Id
                };

                dataContext.Save(gamingGroup, testUserWithDefaultGamingGroup);
                dataContext.CommitAllChanges();

                int actualId = gamingGroup.Id;
                Cleanup(dataContext, gamingGroup, testUserWithDefaultGamingGroup);

                Assert.AreNotEqual(default(int), gamingGroup.Id);
            }
        }

        private static void Cleanup(
            NemeStatsDataContext dbContext, 
            GamingGroup gamingGroup, 
            ApplicationUser currentUser)
        {
            GamingGroup gamingGroupToDelete = dbContext.GetQueryable<GamingGroup>()
                .Where(game => game.Name == gamingGroup.Name).FirstOrDefault();
            if (gamingGroupToDelete != null)
            {
                dbContext.Delete(gamingGroupToDelete, currentUser);
                dbContext.CommitAllChanges();
            }
        }
    }
}
