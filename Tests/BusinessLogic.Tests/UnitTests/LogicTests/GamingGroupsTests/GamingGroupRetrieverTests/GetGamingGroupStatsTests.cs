using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupStatsTests : GamingGroupRetrieverTestBase
    {
        private int _gamingGroupId = 1;
        private int _gameDefinitionIdForOnePlayedGame = 20;
        private int _gameDefinitionIdForTwoPlayedGames = 21;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var queryable = new List<PlayedGame>
            {
                new PlayedGame
                {
                    GameDefinitionId = _gameDefinitionIdForTwoPlayedGames,
                    GamingGroupId = _gamingGroupId
                },
                new PlayedGame
                {
                    GameDefinitionId = _gameDefinitionIdForTwoPlayedGames,
                    GamingGroupId = _gamingGroupId
                },
                new PlayedGame{
                    GameDefinitionId = 1,
                    GamingGroupId = 999
                },
                new PlayedGame{
                    GameDefinitionId = _gameDefinitionIdForOnePlayedGame,
                    GamingGroupId = _gamingGroupId
                }
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(queryable);
        }

        [Test]
        public void It_Returns_The_Number_Of_Total_Played_Games()
        {
            //--arrange

            //--act
            var result = AutoMocker.ClassUnderTest.GetGamingGroupStats(_gamingGroupId);

            //--assert
            result.TotalPlayedGames.ShouldBe(3);
            result.DistinctGamesPlayed.ShouldBe(2);
        }

        [Test]
        public void It_A_NullGamingGroupStats_If_The_Gaming_Group_Id_Isnt_For_A_Real_Gaming_Group()
        {
            //--arrange
            var result = AutoMocker.ClassUnderTest.GetGamingGroupStats(-1);

            //--assert
            result.ShouldBeSameAs(GamingGroupStats.NullStats);
        }

    }
}
