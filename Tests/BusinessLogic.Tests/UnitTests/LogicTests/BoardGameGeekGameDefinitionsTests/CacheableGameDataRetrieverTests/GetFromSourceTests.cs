using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekGameDefinitionsTests.CacheableGameDataRetrieverTests
{
    public class CacheableGameDataRetrieverTests
    {
        protected RhinoAutoMocker<CacheableGameDataRetriever> AutoMocker;

        private int _boardGameGeekGameDefinitionId = 1;
        private BoardGameGeekGameDefinition _expectedBoardGameGeekGameDefinition;
        private string _mechanic1 = "first mechanic";
        private string _mechanic2 = "first mechanic";
        private string _category1 = "first category";
        private string _category2 = "second category";

        [SetUp]
        public void SetUp()
        {
            AutoMocker = new RhinoAutoMocker<CacheableGameDataRetriever>();

            _expectedBoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                AverageWeight = 100,
                MinPlayTime = 1,
                MaxPlayTime = 3,
                Id = _boardGameGeekGameDefinitionId,
                MaxPlayers = 200,
                MinPlayers = 201,
                Name = "some game name",
                Description = "some game description",
                Thumbnail = "some thumbnail url",
                Image = "some image url",
                YearPublished = 2099,
                Mechanics = new List<BoardGameGeekGameMechanic>
                {
                    new BoardGameGeekGameMechanic
                    {
                        MechanicName = _mechanic1
                    },
                    new BoardGameGeekGameMechanic
                    {
                        MechanicName = _mechanic2
                    }
                },
                Categories = new List<BoardGameGeekGameCategory>
                {
                    new BoardGameGeekGameCategory
                    {
                        CategoryName = _category1
                    },
                    new BoardGameGeekGameCategory
                    {
                        CategoryName = _category2
                    }
                },
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
            }.ToList().AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(queryable);
        }

        public class When_Calling_GetFromSource : CacheableGameDataRetrieverTests
        {

            [Test]
            public void It_Throws_An_Entity_Does_Not_Exist_Exception_If_The_BoardGameGeekGameDefinitionId_Isnt_Valid()
            {
                //--arrange
                int invalidId = -1;
                var expectedException = new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), invalidId);

                //--act
                var exception = Assert.Throws<EntityDoesNotExistException>(() => AutoMocker.ClassUnderTest.GetFromSource(invalidId));

                //--assert
                exception.Message.ShouldBe(expectedException.Message);
            }

            [Test]
            public void It_Returns_The_Basic_BGG_Data()
            {
                //--arrange

                //--act
                var result = AutoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId).BoardGameGeekInfo;

                //--assert
                result.BoardGameGeekGameDefinitionId.ShouldBe(_expectedBoardGameGeekGameDefinition.Id);
                result.BoardGameGeekAverageWeight.ShouldBe(_expectedBoardGameGeekGameDefinition.AverageWeight);
                result.BoardGameGeekDescription.ShouldBe(_expectedBoardGameGeekGameDefinition.Description);
                result.BoardGameGeekYearPublished.ShouldBe(_expectedBoardGameGeekGameDefinition.YearPublished);
                result.ImageUrl.ShouldBe(_expectedBoardGameGeekGameDefinition.Image);
                result.MaxPlayers.ShouldBe(_expectedBoardGameGeekGameDefinition.MaxPlayers);
                result.MinPlayers.ShouldBe(_expectedBoardGameGeekGameDefinition.MinPlayers);
                result.MinPlayTime.ShouldBe(_expectedBoardGameGeekGameDefinition.MinPlayTime);
                result.MaxPlayTime.ShouldBe(_expectedBoardGameGeekGameDefinition.MaxPlayTime);
                result.Name.ShouldBe(_expectedBoardGameGeekGameDefinition.Name);
                result.ThumbnailImageUrl.ShouldBe(_expectedBoardGameGeekGameDefinition.Thumbnail);
            }

            [Test]
            public void It_Returns_The_Game_Mechanics()
            {
                //--arrange

                //--act
                var result = AutoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId).BoardGameGeekInfo;

                //--assert
                result.BoardGameGeekMechanics.Count.ShouldBe(2);
                result.BoardGameGeekMechanics.ShouldContain(_mechanic1);
                result.BoardGameGeekMechanics.ShouldContain(_mechanic2);
            }

            [Test]
            public void It_Returns_The_BoardGameGeek_Categories()
            {
                //--arrange

                //--act
                var result = AutoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId).BoardGameGeekInfo;

                //--assert
                result.BoardGameGeekCategories.Count.ShouldBe(2);
                result.BoardGameGeekCategories.ShouldContain(_category1);
                result.BoardGameGeekCategories.ShouldContain(_category2);
            }

            [Test]
            public void It_Returns_The_Average_Players_Per_Game()
            {
                //--arrange

                //--act
                var result = AutoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

                //--assert
                result.AveragePlayersPerGame.ShouldBe(2.5D);
            }

            [Test]
            public void It_Returns_The_Total_Number_Of_Games_Played()
            {
                //--arrange

                //--act
                var result = AutoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

                //--assert
                result.TotalNumberOfGamesPlayed.ShouldBe(2);
            }

            [Test]
            public void It_Returns_The_Total_Number_Of_Gaming_Groups_That_Have_Played_This_Game()
            {
                //--arrange

                //--act
                var result = AutoMocker.ClassUnderTest.GetFromSource(_boardGameGeekGameDefinitionId);

                //--assert
                result.TotalGamingGroupsWithThisGame.ShouldBe(2);
            }
        }
    }
}
