using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models;
using NSubstitute;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekGameDefinitionsTests.UniversalGameRetrieverTests
{
    [TestFixture]
    public class GetAllActiveBoardGameGeekGameDefinitionSitemapInfosTests
    {
        private RhinoAutoMocker<UniversalGameRetriever> _autoMocker;
        private BoardGameGeekGameDefinition _gameWithPlays;
        private BoardGameGeekGameDefinition _gameWithNoPlays;
        private DateTime _mostRecentPlayedGame = DateTime.UtcNow.Date;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalGameRetriever>();

            _gameWithNoPlays = new BoardGameGeekGameDefinition
            {
                Id = 2,
                DateCreated = DateTime.UtcNow.AddYears(-5),
                GameDefinitions = new List<GameDefinition>
                {
                    new GameDefinition
                    {

                    }
                }
            };

            _gameWithPlays = new BoardGameGeekGameDefinition
            {
                Id = 1,
                GameDefinitions = new List<GameDefinition>
                {
                    new GameDefinition
                    {
                        PlayedGames = new List<PlayedGame>
                        {
                            new PlayedGame
                            {
                                DatePlayed = _mostRecentPlayedGame.AddDays(-4)
                            }
                        }
                    },
                    new GameDefinition
                    {
                        PlayedGames = new List<PlayedGame>
                        {
                            new PlayedGame
                            {
                                DatePlayed = _mostRecentPlayedGame.AddDays(-3)
                            },
                            new PlayedGame
                            {
                                DatePlayed = _mostRecentPlayedGame
                            }
                        }
                    }
                }
            };
            var queryable = new List<BoardGameGeekGameDefinition>
            {
                _gameWithNoPlays,
                _gameWithPlays
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(queryable);
        }

        [Test]
        public void It_Returns_All_Games_With_The_Date_Of_The_Last_Game_Played_And_Date_Created()
        {
            //--arrange

            //--act
            var results = _autoMocker.ClassUnderTest.GetAllActiveBoardGameGeekGameDefinitionSitemapInfos();

            //--assert
            results.ShouldNotBeNull();
            results.Count.ShouldBe(2);
            var gameWithNoPlays = results.First(x => x.BoardGameGeekGameDefinitionId == _gameWithNoPlays.Id);
            gameWithNoPlays.ShouldNotBeNull();
            gameWithNoPlays.DateLastGamePlayed.ShouldBe(DateTime.MinValue);
            gameWithNoPlays.DateCreated.ShouldBe(_gameWithNoPlays.DateCreated);

            var gameWithPlays = results.First(x => x.BoardGameGeekGameDefinitionId == _gameWithPlays.Id);
            gameWithPlays.DateLastGamePlayed.ShouldBe(_mostRecentPlayedGame);
            gameWithPlays.DateCreated.ShouldBe(_gameWithPlays.DateCreated);
        }

        [Test]
        public void It_Returns_Results_Ordered_By_BoardGameGeekGameDefinitionId()
        {
            //--arrange

            //--act
            var results = _autoMocker.ClassUnderTest.GetAllActiveBoardGameGeekGameDefinitionSitemapInfos();

            //--assert
            results[0].BoardGameGeekGameDefinitionId.ShouldBeLessThan(results[1].BoardGameGeekGameDefinitionId);
        }


    }
}
