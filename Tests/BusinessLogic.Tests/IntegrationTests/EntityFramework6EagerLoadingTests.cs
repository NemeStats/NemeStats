using System.Data.Entity;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests
{
    //TODO blog post on eager vs. lazy loading, integration testing your eager loads, and entity framework gotchas
    [TestFixture]
    public class EntityFramework6LazyLoadingTests
    {
        [Test, Ignore("I disabled proxy creation and lazy loading on NemeStatsDbContext in the constructor so this would fail now.")]
        public void ItLazyLoadsByDefault()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                Player player2 = dbContext.Players.First();

                //Accessing the PlayerGameResults property will lazy load these entities
                Assert.NotNull(player2.PlayerGameResults);
            }
        }

        [Test]
        public void ItDoesNotLazyLoadIfYouDisableLazyLoadingBeforeExecutingAQuery()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                dbContext.Configuration.LazyLoadingEnabled = false;
                Player player2 = dbContext.Players.First();

                //with lazy loading disabled, PlayerGameResults will not result in a query to the database
                Assert.Null(player2.PlayerGameResults);
            }
        }

        [Test, Ignore("I disabled proxy creation and lazy loading on NemeStatsDbContext in the constructor so this would fail now.")]
        public void ItPullsObjectsFromMemoryIfTheyHaveAlreadyBeenLoaded()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                //normal query with no lazy loading (since I'm not accessing any of the properties of related entities)
                Player player = dbContext.Players.First();
                //accessing PlayerGameResults will cause this data to be lazy loaded
                player.PlayerGameResults.First();
                dbContext.Configuration.LazyLoadingEnabled = false;

                Player player2 = dbContext.Players.First();

                //even though LazyLoadingEnabled is false, it will fetch since this player has already been loaded into
                // memory before
                Assert.NotNull(player2.PlayerGameResults);
            }
        }


        [Test, Ignore("Integration test")]
        public void EagerLoadTest()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                var result = (from gameDefinition in dataContext.GetQueryable<GameDefinition>()
                                                                .Include(game => game.Champion.Player)
                              where gameDefinition.Id == 2004
                              select gameDefinition
                    /*select new {
                        Champion = gameDefinition.Champion
                        }*/).First();

                Assert.That(result.Champion, Is.Not.Null);
                Assert.That(result.Champion.Player, Is.Not.Null);
            }
        }

        [Test(Description = "Have to manually inspect SQL to see what the query looks like"), Ignore("Integration test")]
        public void ItOnlyFetchesRelatedEntitiesThatAreNeeded()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                var result = (from gamingGroup in dbContext.GamingGroups.Include(g => g.OwningUser.Players.Select(p => p.Nemesis))
                              where gamingGroup.Id == 1
                              select gamingGroup.OwningUser.Players.Select(p => p.Nemesis)).ToList();
                ;
            }
        }
    }
}
