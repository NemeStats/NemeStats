using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetTopGamesTests : GameDefinitionRetrieverTestBase
    {
        [SetUp]
        public void LocalSetUp()
        {
            
        }

        [Test]
        public void ItOnlyReturnsCountsForPlayedGamesThatHappenedXDaysAgoOrLater()
        {
            int days = 1;
            string expectedThumbnail = "some thumbnail";

            var expectedPlayedGame = new PlayedGame
            {
                DatePlayed = DateTime.Now.Date.AddDays(days * -1),
            };
            var excludedPlayedGame = new PlayedGame
            {
                DatePlayed = DateTime.Now.Date.AddDays((days + 1) * -1),
                GameDefinition = new GameDefinition
                {
                    BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition()
                }
            };
            var playedGames = new List<PlayedGame>
            {
                expectedPlayedGame,
                excludedPlayedGame
            };
            var boardGameGeekGameDefinitionQueryable = new List<BoardGameGeekGameDefinition>
            {
                new BoardGameGeekGameDefinition
                {
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = playedGames
                        }
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(boardGameGeekGameDefinitionQueryable);

            var results = autoMocker.ClassUnderTest.GetTopGames(100, days);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].GamesPlayed, Is.EqualTo(1));
        }

        [Test]
        public void ItOnlyReturnsTheSpecifiedNumberOfTopGamesOrderedByGamesPlayed()
        {
            int expectedBoardGameGeekGameDefinition1 = 1;
            int expectedBoardGameGeekGameDefinition2 = 2;

            var playedGames1 = new List<PlayedGame>
            {
                new PlayedGame(),
                new PlayedGame(),
                new PlayedGame()
            };

            var playedGames2 = new List<PlayedGame>
            {
                new PlayedGame(),
                new PlayedGame()
            };
            var playedGames3 = new List<PlayedGame>
            {
                new PlayedGame()
            };
            var boardGameGeekGameDefinitionQueryable = new List<BoardGameGeekGameDefinition>
            {
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinition2,
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = playedGames2
                        }
                    }
                },
                new BoardGameGeekGameDefinition
                {
                    Id = -1,
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = playedGames3
                        }
                    }
                },
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinition1,
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = playedGames1
                        }
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(boardGameGeekGameDefinitionQueryable);
            int expectedNumberOfGames = 2;

            var results = autoMocker.ClassUnderTest.GetTopGames(expectedNumberOfGames, 1);

            Assert.That(results.Count, Is.EqualTo(expectedNumberOfGames));
            Assert.That(results[0].GamesPlayed, Is.EqualTo(3));
            Assert.That(results[0].BoardGameGeekGameDefinitionId, Is.EqualTo(expectedBoardGameGeekGameDefinition1));
            Assert.That(results[1].GamesPlayed, Is.EqualTo(2));
            Assert.That(results[1].BoardGameGeekGameDefinitionId, Is.EqualTo(expectedBoardGameGeekGameDefinition2));
        }

        [Test]
        public void ItSetsTheCorrectFields()
        {
            int expectedBoardGameGeekGameDefinitionId1 = 1;
            var expectedPlayedGame = new PlayedGame
            {
                Id = 1
            };
            var playedGames = new List<PlayedGame>
            {
                expectedPlayedGame,
                expectedPlayedGame
            };
            string expectedName = "some game definition name";
            string expectedThumbnail = "some expected thumbnail";

            var boardGameGeekGameDefinitionQueryable = new List<BoardGameGeekGameDefinition>
            {
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinitionId1,
                    Name = expectedName,
                    Thumbnail = expectedThumbnail,
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = playedGames
                        }
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(boardGameGeekGameDefinitionQueryable);
            int expectedNumberOfGames = 2;

            var results = autoMocker.ClassUnderTest.GetTopGames(expectedNumberOfGames, 1);

            Assert.That(results[0].BoardGameGeekGameDefinitionId, Is.EqualTo(expectedBoardGameGeekGameDefinitionId1));
            Assert.That(results[0].BoardGameGeekUri, Is.EqualTo(BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(expectedBoardGameGeekGameDefinitionId1)));
            Assert.That(results[0].GamingGroupsPlayingThisGame, Is.EqualTo(1));
            Assert.That(results[0].GamesPlayed, Is.EqualTo(2));
            Assert.That(results[0].Name, Is.EqualTo(expectedName));
            Assert.That(results[0].ThumbnailImageUrl, Is.EqualTo(expectedThumbnail));
        }
    }
}
