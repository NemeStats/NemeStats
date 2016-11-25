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

        private Champion _topChampion;
        private Champion _champion2;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalTopChampionsRetreiver>();

            _topChampion = new Champion
            {
                GameDefinitionId = 1,
                WinPercentage = 100,
                NumberOfWins = 2,
                NumberOfGames = 2,
                PlayerId = 100,
                Player = new Player()
                {
                    Name = "player name 1",
                    GamingGroupId = 200,
                    GamingGroup = new GamingGroup
                    {
                        Name = "gaming group 1"
                    }
                }
            };
            _champion2 = new Champion
            {
                GameDefinitionId = 2,
                WinPercentage = 50,
                NumberOfWins = 1,
                NumberOfGames = 2,
                PlayerId = 101,
                Player = new Player()
                {
                    Name = "player name 2",
                    GamingGroupId = 201,
                    GamingGroup = new GamingGroup
                    {
                        Name = "gaming group 2"
                    }
                }
            };

            var expectedGameDefinition = new GameDefinition
            {
                Id = 20,
                BoardGameGeekGameDefinitionId = _boardGameGeekGameDefinitionId,
                Champion = _topChampion
            };

            var otherGameDefinition = new GameDefinition
            {
                Id = 21,
                BoardGameGeekGameDefinitionId = _boardGameGeekGameDefinitionId,
                Champion = _champion2
            };


            var gameDefinitionQueryable = new List<GameDefinition>
            {
                expectedGameDefinition,
                otherGameDefinition
            }.AsQueryable();

            
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);
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
        public void It_Returns_Top_Champions()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

            //--assert
            result.Count.ShouldBe(2);
            result.First().PlayerId.ShouldBe(_topChampion.PlayerId);
        }
        
    }
}
