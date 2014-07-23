using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
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
            using(ApplicationDbContext dbContext = new ApplicationDbContext(new SecuredEntityValidatorFactory()))
            {
                GamingGroup gamingGroup = new GamingGroup()
                {
                    Name = "new gaming group without an ID yet",
                    OwningUserId = testUserWithDefaultGamingGroup.Id
                };

                dbContext.GamingGroups.AddOrUpdate(gamingGroup);
                dbContext.SaveChanges();

                int actualId = gamingGroup.Id;
                Cleanup(dbContext, gamingGroup);

                Assert.AreNotEqual(default(int), gamingGroup.Id);
            }
        }

        private static void Cleanup(ApplicationDbContext dbContext, GamingGroup gamingGroup)
        {
            GamingGroup gamingGroupToDelete = dbContext.GamingGroups.Where(game => game.Name == gamingGroup.Name).FirstOrDefault();
            if (gamingGroupToDelete != null)
            {
                dbContext.GamingGroups.Remove(gamingGroupToDelete);
                dbContext.SaveChanges();
            }
        }
    }
}
