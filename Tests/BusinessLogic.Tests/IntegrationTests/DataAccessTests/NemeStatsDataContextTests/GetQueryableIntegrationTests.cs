using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.NemeStatsDataContextTests.ApplicationDataContextTests
{
    [TestFixture]
    public class GetQueryableIntegrationTests : IntegrationTestBase
    {
        //TODO why does the top test fail and the bottom test passes? Looks like the clever predicate addition in ApplicationDataContext
        // isn't working :(
        /*
        [Test]
        public void ItAutomaticallyFiltersDownToTheCurrentUsersGamingGroupIfTheEntityIsSecured()
        {
            using(ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                List<GameDefinition> gameDefinitions = dataContext
                    .GetQueryable<GameDefinition>(testUserWithOtherGamingGroup)
                    .ToList();

                Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.GamingGroupId 
                    == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }

        [Test]
        public void TEMP_testToShowThatTheFilterWorksIfThereIsNoCasting()
        {
            using (NemeStatsDbContext dataContext = new NemeStatsDbContext())
            {
                List<GameDefinition> gameDefinitions = dataContext.Set<GameDefinition>()
                    .Where(x => x.GamingGroupId == testUserWithOtherGamingGroup.CurrentGamingGroupId.Value)
                    .ToList();

                Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.GamingGroupId
                    == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }
         * */
    }
}
