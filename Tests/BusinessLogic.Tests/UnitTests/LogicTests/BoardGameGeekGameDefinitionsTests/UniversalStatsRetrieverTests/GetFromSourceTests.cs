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

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekGameDefinitionsTests.UniversalStatsRetrieverTests
{
    public class GetFromSourceTests
    {
        private int _boardGameGeekGameDefinitionId = 1;
        private RhinoAutoMocker<UniversalStatsRetriever> _autoMocker;
        private BoardGameGeekGameDefinition _expectedBoardGameGeekGameDefinition;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalStatsRetriever>();

            _expectedBoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                Id = _boardGameGeekGameDefinitionId,
                GameDefinitions = new List<GameDefinition>
                {
                    new GameDefinition
                    {
                        GamingGroupId = 1,
                        PlayedGames = new List<PlayedGame>
                        {
                            new PlayedGame
                            {
                                NumberOfPlayers = 2
                            }
                        }
                    },
                    new GameDefinition
                    {
                        GamingGroupId = 2,
                        PlayedGames = new List<PlayedGame>
                        {
                            new PlayedGame
                            {
                                NumberOfPlayers = 3
                            }
                        }
                    }
                }
            };

            var queryable = new List<BoardGameGeekGameDefinition>
            {
                _expectedBoardGameGeekGameDefinition
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(queryable);
        }

        [Test]
        public void It_Throws_An_EntityDoesNotExistException_If_The_BoardGameGeekId_Doesnt_Map_To_A_Record()
        {
            //--arrange
            int invalidBoardGameGeekGameDefinitionId = -100;
            var expectedException = new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), invalidBoardGameGeekGameDefinitionId);

            //--act
            var exception = Assert.Throws<EntityDoesNotExistException>(() => _autoMocker.ClassUnderTest.GetFromSource(invalidBoardGameGeekGameDefinitionId));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void It_Returns_The_Average_Players_Per_Game()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

            //--assert
            result.AveragePlayersPerGame.ShouldBe(2.5D);
        }

        [Test]
        public void It_Returns_The_Total_Number_Of_Games_Played()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

            //--assert
            result.TotalNumberOfGamesPlayed.ShouldBe(2);
        }

        [Test]
        public void It_Returns_The_Total_Number_Of_Gaming_Groups_That_Have_Played_This_Game()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

            //--assert
            result.TotalGamingGroupsWithThisGame.ShouldBe(2);
        }
    }
}
