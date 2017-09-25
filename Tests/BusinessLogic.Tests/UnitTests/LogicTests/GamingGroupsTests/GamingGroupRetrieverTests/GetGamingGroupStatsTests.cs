using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
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

            var playedGameQueryable = new List<PlayedGame>
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
                    GamingGroupId = -1
                },
                new PlayedGame{
                    GameDefinitionId = _gameDefinitionIdForOnePlayedGame,
                    GamingGroupId = _gamingGroupId
                }
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGameQueryable);

            var gameDefinitionsQueryable = new List<GameDefinition>
            {
                new GameDefinition
                {
                    GamingGroupId = _gamingGroupId
                },
                new GameDefinition
                {
                    GamingGroupId = _gamingGroupId
                },
                new GameDefinition
                {
                    GamingGroupId = -1
                }
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionsQueryable);
        }

        [Test]
        public void It_Returns_The_Number_Of_Total_Played_Games_And_Total_Number_Of_Game_Definitions()
        {
            //--arrange
            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());

            //--act
            var result = AutoMocker.ClassUnderTest.GetGamingGroupStats(_gamingGroupId);

            //--assert
            result.TotalPlayedGames.ShouldBe(3);
            result.TotalNumberOfGamesWithPlays.ShouldBe(2);
        }

        [Test]
        public void It_Returns_The_Total_Number_Of_Players_And_Players_With_Plays()
        {
            //--arrange
            var playersQueryable = new List<Player>
            {
                new Player
                {
                    Id = 1,
                    GamingGroupId = _gamingGroupId,
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult()
                    }
                },
                new Player
                {
                    Id = 2,
                    GamingGroupId = _gamingGroupId,
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult()
                    }
                },
                //--player without played games
                new Player
                {
                    Id = 3,
                    GamingGroupId = _gamingGroupId,
                    PlayerGameResults = new List<PlayerGameResult>()
                },
                //--bad gaming group id to make sure filter works
                new Player
                {
                    Id = 1,
                    GamingGroupId = -1
                }
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(playersQueryable);

            //--act
            var result = AutoMocker.ClassUnderTest.GetGamingGroupStats(_gamingGroupId);

            //--assert
            result.TotalNumberOfPlayersWithPlays.ShouldBe(2);
            result.TotalNumberOfPlayers.ShouldBe(3);
        }

        [Test]
        public void It_Returns_The_Total_Number_Of_Games_Owned()
        {
            //--arrange
            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());

            //--act
            var result = AutoMocker.ClassUnderTest.GetGamingGroupStats(_gamingGroupId);

            //--assert
            result.TotalGamesOwned.ShouldBe(2);
        }
    }
}
