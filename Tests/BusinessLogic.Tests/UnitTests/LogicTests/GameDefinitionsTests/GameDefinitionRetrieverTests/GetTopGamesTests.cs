using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
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
        public void ItOnlyReturnsCountsForGamesPlayedWithinTheSpecifiedNumberOfDays()
        {
            int days = 1;
            string expectedThumbnail = "some thumbnail";
            var expectedPlayedGame = new PlayedGame
            {
                DatePlayed = DateTime.Now.Date.AddDays(days * -1),
                GameDefinition = new GameDefinition
                {
                    BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
                    {
                         Thumbnail = expectedThumbnail
                    }
                }
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
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGames);

            var results = autoMocker.ClassUnderTest.GetTopGames(1);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].ThumbnailImageUrl, Is.EqualTo(expectedPlayedGame));
        }

    }
}
