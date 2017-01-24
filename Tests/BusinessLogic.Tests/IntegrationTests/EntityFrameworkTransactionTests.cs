using System;
using System.Data.Entity.Migrations;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class EntityFrameworkTransactionTests : IntegrationTestIoCBase
    {
        private int _someGamingGroupId;

        [OneTimeSetUp]
        public void LocalOneTimeSetup()
        {
            using (var dataContext = GetInstanceFromRootContainer<IDataContext>())
            {
                _someGamingGroupId = dataContext.GetQueryable<GamingGroup>().First().Id;
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DisposeNestedContainer();
        }

        [Test]
        public void Entity_Framework_Demonstration_Of_Transactions_And_Entity_Ids()
        {
            //--create a DB context
            using (var dbContext = GetInstanceFromRootContainer<NemeStatsDbContext>())
            {
                //--create a transaction from the DB context
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    //--some entity I'm going to save
                    var player = new Player
                    {
                        Name = "some unique player name for this integration test",
                        GamingGroupId = _someGamingGroupId
                    };
                    
                    dbContext.Set<Player>().AddOrUpdate(player);
                    int playerId = player.Id;
                    //--the Id (PK field of the entity) is not set yet since we haven't called SaveChanges()
                    playerId.ShouldBe(default(int));
                    dbContext.SaveChanges();
                    int playerIdAfterSaveChanges = player.Id;
                    //--since we called SaveChanges, the player.Id was populated
                    playerIdAfterSaveChanges.ShouldNotBe(0);

                    transaction.Rollback();

                    //--since we did a .Rollback() on the transaction, the entity is not in the DB, even though we called .SaveChanges()
                    var playerAfterRollback = dbContext.Set<Player>().FirstOrDefault(x => x.Id == playerId);

                    playerAfterRollback.ShouldBeNull();
                }
            }
        }

        [Test]
        public void It_Rolls_Back_Anything_That_Wasnt_Explicitly_Committed_On_The_Transaction_When_Disposing_The_Transaction()
        {
            var newPlayer = new Player
            {
                Name = Guid.NewGuid().ToString(),
                GamingGroupId = _someGamingGroupId,
                Active = false
            };

            //--get an instance from the root container since it's OK to dispose transient instances
            //--create a DB context
            using (var dbContext = GetInstanceFromRootContainer<NemeStatsDbContext>())
            {
                //--create a transaction from the DB context but just let it get disposed
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    dbContext.Set<Player>().Add(newPlayer);
                    dbContext.SaveChanges();
                }
            }

            using (var dataContext = GetInstanceFromRootContainer<IDataContext>())
            {
                var playerAfterDisposed = dataContext.GetQueryable<Player>()
                    .FirstOrDefault(x => x.Name == newPlayer.Name);

                playerAfterDisposed.ShouldBeNull();
            }
        }
    }
}
