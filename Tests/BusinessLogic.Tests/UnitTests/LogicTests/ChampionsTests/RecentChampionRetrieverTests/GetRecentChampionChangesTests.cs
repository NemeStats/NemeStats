using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.ChampionsTests.RecentChampionRetrieverTests
{
    [TestFixture]
    public class GetRecentChampionChangesTests
    {
        private RhinoAutoMocker<RecentChampionRetriever> _autoMocker;
        private const int GAMING_GROUP_ID = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<RecentChampionRetriever>();
        }

        private GameDefinition MakeValidGameDefinition(int gameDefinitionId = 1)
        {
            var gameDefinition = new GameDefinition
            {
                GamingGroupId = GAMING_GROUP_ID,
                Active = true,
                Id = gameDefinitionId,
                Name = "game definition name " + gameDefinitionId,
                ChampionId = 0,
                Champion = new Champion
                {
                    DateCreated = DateTime.UtcNow.Date,
                    PlayerId = 1000 + gameDefinitionId,
                    Player = new Player
                    {
                        Name = "new champion of game definition" + gameDefinitionId
                    }
                },
                PreviousChampion = new Champion
                {
                    PlayerId = 1001 + gameDefinitionId,
                    Player = new Player
                    {
                        Name = "previous champion of game definition" + gameDefinitionId
                    }
                }
            };
            return gameDefinition;
        }

        [Test]
        public void It_Returns_Recent_Champion_Changes_Ordered_By_Champion_Created_Date_Descending()
        {
            //--arrange
            var gameDefinitionThatMatchesWithOlderDate = MakeValidGameDefinition(2);
            gameDefinitionThatMatchesWithOlderDate.Champion.DateCreated = DateTime.UtcNow.Date.AddDays(-1 * 100);
            gameDefinitionThatMatchesWithOlderDate.Champion.PlayerId = 20;

            var gameDefinitionThatMatchesWithNewerDate = MakeValidGameDefinition(1);
            var queryable = new List<GameDefinition>
            {
                gameDefinitionThatMatchesWithOlderDate,
                gameDefinitionThatMatchesWithNewerDate
                
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(queryable);

            var filter = new GetRecentChampionChangesFilter(GAMING_GROUP_ID, 100);

            //--act
            var results = _autoMocker.ClassUnderTest.GetRecentChampionChanges(filter);

            //--assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(2);

            //--first result is the newest
            results[0].DateCreated.Date.ShouldBe(gameDefinitionThatMatchesWithNewerDate.Champion.DateCreated);
            results[0].GameDefinitionId.ShouldBe(gameDefinitionThatMatchesWithNewerDate.Id);
            results[0].GameName.ShouldBe(gameDefinitionThatMatchesWithNewerDate.Name);
            results[0].NewChampionPlayerId.ShouldBe(gameDefinitionThatMatchesWithNewerDate.Champion.PlayerId);
            results[0].NewChampionPlayerName.ShouldBe(gameDefinitionThatMatchesWithNewerDate.Champion.Player.Name);
            results[0].PreviousChampionPlayerId.ShouldBe(gameDefinitionThatMatchesWithNewerDate.PreviousChampion.PlayerId);
            results[0].PreviousChampionPlayerName.ShouldBe(gameDefinitionThatMatchesWithNewerDate.PreviousChampion.Player.Name);

            //--second result is the oldest
            results[1].DateCreated.Date.ShouldBe(gameDefinitionThatMatchesWithOlderDate.Champion.DateCreated);
            results[1].GameDefinitionId.ShouldBe(gameDefinitionThatMatchesWithOlderDate.Id);
            results[1].GameName.ShouldBe(gameDefinitionThatMatchesWithOlderDate.Name);
            results[1].NewChampionPlayerId.ShouldBe(gameDefinitionThatMatchesWithOlderDate.Champion.PlayerId);
            results[1].NewChampionPlayerName.ShouldBe(gameDefinitionThatMatchesWithOlderDate.Champion.Player.Name);
            results[1].PreviousChampionPlayerId.ShouldBe(gameDefinitionThatMatchesWithOlderDate.PreviousChampion.PlayerId);
            results[1].PreviousChampionPlayerName.ShouldBe(gameDefinitionThatMatchesWithOlderDate.PreviousChampion.Player.Name);
        }

        [Test]
        public void It_Applies_Appropriate_Filters_When_Returning_Results()
        {
            //--arrange
            var gameDefinitionWithBadGamingGroupId = MakeValidGameDefinition();
            gameDefinitionWithBadGamingGroupId.GamingGroupId = -1;

            var gameDefinitionThatIsNotActive = MakeValidGameDefinition();
            gameDefinitionThatIsNotActive.Active = false;

            var gameDefinitionThatHasNoChampion = MakeValidGameDefinition();
            gameDefinitionThatHasNoChampion.ChampionId = null;


            var queryable = new List<GameDefinition>
            {
                gameDefinitionWithBadGamingGroupId,
                gameDefinitionThatIsNotActive,
                gameDefinitionThatHasNoChampion

            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(queryable);

            var filter = new GetRecentChampionChangesFilter(GAMING_GROUP_ID, 100);

            //--act
            var results = _autoMocker.ClassUnderTest.GetRecentChampionChanges(filter);

            //--assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(0);
        }

        [Test]
        public void It_Takes_Only_The_Specified_Number_Of_Results()
        {
            //--arrange
            var gameDefinitionThatIsOlderAndWontGetPulled = MakeValidGameDefinition(1);
            gameDefinitionThatIsOlderAndWontGetPulled.Champion.DateCreated = DateTime.UtcNow.AddDays(-100);
            var gameDefinitionIsNewerAndShouldGetPulled = MakeValidGameDefinition(2);

            var queryable = new List<GameDefinition>
            {
                gameDefinitionThatIsOlderAndWontGetPulled,
                gameDefinitionIsNewerAndShouldGetPulled

            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(queryable);

            var filter = new GetRecentChampionChangesFilter(GAMING_GROUP_ID, 1);

            //--act
            var results = _autoMocker.ClassUnderTest.GetRecentChampionChanges(filter);

            //--assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(1);
            results.ShouldContain(x => x.GameDefinitionId == gameDefinitionIsNewerAndShouldGetPulled.Id);
        }
    }
}
