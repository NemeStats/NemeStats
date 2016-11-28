using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekGameDefinitionsTests.UniversalTopChampionsRetreiverTests
{
    public class GetFromSourceTests
    {
        private int _boardGameGeekGameDefinitionId = 1;
        private RhinoAutoMocker<UniversalTopChampionsRetreiver> _autoMocker;
        private BoardGameGeekGameDefinition _expectedBoardGameGeekGameDefinition;
        private GameDefinition _expectedFirstChampionGameDefinition;

        private int _topChampionId = 50;
        private int _champion2Id = 51;
        private int _invalidChampionId = 52;
        private Champion _topChampion;
        private Champion _champion2;
        private Champion _championThatIsNoLongerTheCurrentChampionForTheGameDefinition;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalTopChampionsRetreiver>();

            _expectedFirstChampionGameDefinition = new GameDefinition
            {
                Id = 20,
                BoardGameGeekGameDefinitionId = _boardGameGeekGameDefinitionId,
                ChampionId = _topChampionId
            };

            var otherGameDefinition = new GameDefinition
            {
                Id = 21,
                BoardGameGeekGameDefinitionId = _boardGameGeekGameDefinitionId,
                ChampionId = _champion2Id
            };

            _topChampion = new Champion
            {
                Id = _topChampionId,
                GameDefinitionId = _expectedFirstChampionGameDefinition.Id,
                WinPercentage = 100,
                NumberOfWins = 2,
                NumberOfGames = 2,
                PlayerId = 200,
                Player = new Player()
                {
                    Name = "player name 1",
                    GamingGroupId = 200,
                    GamingGroup = new GamingGroup
                    {
                        Name = "gaming group 1"
                    }
                },
                GameDefinition = _expectedFirstChampionGameDefinition
            };
            _champion2 = new Champion
            {
                Id = _champion2Id,
                GameDefinitionId = otherGameDefinition.Id,
                WinPercentage = 50,
                NumberOfWins = 1,
                NumberOfGames = 2,
                PlayerId = 201,
                Player = new Player()
                {
                    Name = "player name 2",
                    GamingGroupId = 201,
                    GamingGroup = new GamingGroup
                    {
                        Name = "gaming group 2"
                    }
                },
                GameDefinition = otherGameDefinition
            };
            _championThatIsNoLongerTheCurrentChampionForTheGameDefinition = new Champion
            {
                Id = -1,
                GameDefinitionId = otherGameDefinition.Id,
                WinPercentage = 50,
                NumberOfWins = 1,
                NumberOfGames = 2,
                PlayerId = 202,
                Player = new Player()
                {
                    Name = "player name 2",
                    GamingGroupId = 201,
                    GamingGroup = new GamingGroup
                    {
                        Name = "gaming group 2"
                    }
                },
                GameDefinition = otherGameDefinition
            };

            var championQueryable = new List<Champion>
            {
                _topChampion,
                _champion2,
                _championThatIsNoLongerTheCurrentChampionForTheGameDefinition
            }.AsQueryable();
        
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>()).Return(championQueryable);
        }

        [Test]
        public void It_Returns_Empty_If_BoardGameGeekId_Doesnt_Map_To_A_Record()
        {
            //--arrange
            int invalidBoardGameGeekGameDefinitionId = -100;

            //--act
            var result = _autoMocker.ClassUnderTest.GetFromSource(invalidBoardGameGeekGameDefinitionId);

            //--assert
            result.ShouldBeEmpty();
        }

        [Test]
        public void It_Returns_Top_Champions_For_Active_GameDefinitions_Only()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

            //--assert
            result.Count.ShouldBe(2);
            var firstChampion = result[0];
            firstChampion.PlayerId.ShouldBe(_topChampion.PlayerId);
            firstChampion.GameDefinitionId.ShouldBe(_expectedFirstChampionGameDefinition.Id);
            firstChampion.NumberOfGames.ShouldBe(_topChampion.NumberOfGames);
            firstChampion.NumberOfWins.ShouldBe(_topChampion.NumberOfWins);
            firstChampion.PlayerGamingGroupId.ShouldBe(_topChampion.Player.GamingGroupId);
            firstChampion.PlayerGamingGroupName.ShouldBe(_topChampion.Player.GamingGroup.Name);
            firstChampion.PlayerName.ShouldBe(_topChampion.Player.Name);
            firstChampion.WinPercentage.ShouldBe(100 * _topChampion.NumberOfWins / _topChampion.NumberOfGames);
            var secondChampion = result[1];
            secondChampion.PlayerId.ShouldBe(_champion2.PlayerId);
        }
    }
}
