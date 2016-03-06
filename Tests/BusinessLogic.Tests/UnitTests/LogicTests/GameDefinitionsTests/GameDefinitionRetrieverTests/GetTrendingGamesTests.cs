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
    public class GetTrendingGamesTests : GameDefinitionRetrieverTestBase
    {
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
                        //--this has one included played game
                        new GameDefinition
                        {
                            PlayedGames = playedGames
                        },
                        //--this has one included played game
                        new GameDefinition
                        {
                            PlayedGames = playedGames
                        },
                        new GameDefinition
                        {
                            PlayedGames = new List<PlayedGame>
                            {
                                excludedPlayedGame
                            }
                        }
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(boardGameGeekGameDefinitionQueryable);

            var results = autoMocker.ClassUnderTest.GetTrendingGames(100, days);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].GamesPlayed, Is.EqualTo(2));
            Assert.That(results[0].GamingGroupsPlayingThisGame, Is.EqualTo(2));
        }

        [Test]
        public void ItOnlyReturnsTheSpecifiedNumberOfTopGamesOrderedByTheNumberOfGamingGroupsThatPlayedTheGameThenNumberOfPlayedGames()
        {
            int expectedBoardGameGeekGameDefinition1 = 1;
            int expectedBoardGameGeekGameDefinition2 = 2;

            var threePlayedGames = new List<PlayedGame>
            {
                new PlayedGame(),
                new PlayedGame(),
                new PlayedGame()
            };

            var twoPlayedGames = new List<PlayedGame>
            {
                new PlayedGame(),
                new PlayedGame()
            };
            var onePlayedGame = new List<PlayedGame>
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
                            PlayedGames = threePlayedGames
                        },
                        new GameDefinition
                        {
                            PlayedGames = twoPlayedGames
                        }
                    }
                },
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinition2,
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = twoPlayedGames
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
                            PlayedGames = onePlayedGame
                        }
                    }
                },
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinition1,
                    //--2 game definitions means 2 separate gaming groups have this game
                    GameDefinitions = new List<GameDefinition>
                    {
                        new GameDefinition
                        {
                            PlayedGames = threePlayedGames
                        },
                        new GameDefinition
                        {
                            PlayedGames = threePlayedGames
                        }
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>()).Return(boardGameGeekGameDefinitionQueryable);
            int expectedNumberOfGames = 2;

            var results = autoMocker.ClassUnderTest.GetTrendingGames(expectedNumberOfGames, 1);

            Assert.That(results.Count, Is.EqualTo(expectedNumberOfGames));
            Assert.That(results[0].GamesPlayed, Is.EqualTo(6));
            Assert.That(results[0].GamingGroupsPlayingThisGame, Is.EqualTo(2));
            Assert.That(results[0].BoardGameGeekGameDefinitionId, Is.EqualTo(expectedBoardGameGeekGameDefinition1));
            Assert.That(results[1].GamesPlayed, Is.EqualTo(5));
            Assert.That(results[1].GamingGroupsPlayingThisGame, Is.EqualTo(2));
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

            var results = autoMocker.ClassUnderTest.GetTrendingGames(expectedNumberOfGames, 1);

            Assert.That(results[0].BoardGameGeekGameDefinitionId, Is.EqualTo(expectedBoardGameGeekGameDefinitionId1));
            Assert.That(results[0].BoardGameGeekUri, Is.EqualTo(BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(expectedBoardGameGeekGameDefinitionId1)));
            Assert.That(results[0].GamingGroupsPlayingThisGame, Is.EqualTo(1));
            Assert.That(results[0].GamesPlayed, Is.EqualTo(2));
            Assert.That(results[0].Name, Is.EqualTo(expectedName));
            Assert.That(results[0].ThumbnailImageUrl, Is.EqualTo(expectedThumbnail));
        }
    }
}
