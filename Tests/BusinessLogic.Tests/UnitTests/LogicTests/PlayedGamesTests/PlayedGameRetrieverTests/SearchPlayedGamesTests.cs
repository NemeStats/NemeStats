using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameRetrieverTests
{
    [TestFixture]
    public class SearchPlayedGamesTests
    {
        private RhinoAutoMocker<PlayedGameRetriever> autoMocker;
        private List<PlayedGame> playedGames;
        private const int PLAYED_GAME_ID_FOR_GAME_RECORDED_IN_MARCH = 1;
        private const int PLAYED_GAME_ID_FOR_GAME_RECORDED_IN_APRIL = 2;
            
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayedGameRetriever>();

            playedGames = new List<PlayedGame>
            {
                new PlayedGame
                {
                    Id = PLAYED_GAME_ID_FOR_GAME_RECORDED_IN_MARCH,
                    DateCreated = new DateTime(2015, 3, 1),
                    PlayerGameResults = new List<PlayerGameResult>(),
                    GameDefinition = new GameDefinition(),
                    GamingGroup = new GamingGroup()
                },
                new PlayedGame
                {
                    Id = PLAYED_GAME_ID_FOR_GAME_RECORDED_IN_APRIL,
                    DateCreated = new DateTime(2015, 4, 1),
                    PlayerGameResults = new List<PlayerGameResult>(),
                    GameDefinition = new GameDefinition(),
                    GamingGroup = new GamingGroup()
                }
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGames.AsQueryable());
        }

        [Test]
        public void ItSetsAllTheFields()
        {
            autoMocker = new RhinoAutoMocker<PlayedGameRetriever>();
            var playerGameResults = new List<PlayerGameResult>
            {
                new PlayerGameResult
                {
                    GameRank = 1,
                    PlayerId = 2,
                    PointScored = 50
                }
            };

            var playedGame = new PlayedGame
            {
                Id = 1,
                DateCreated = new DateTime(2015, 11, 1),
                DatePlayed = new DateTime(2015, 10, 1),
                GameDefinitionId = 2,
                GamingGroupId = 3,
                Notes = "some notes",
                GamingGroup = new GamingGroup
                {
                    Name = "some gaming group name"
                },
                GameDefinition = new GameDefinition
                {
                    Name = "some game definition name",
                    BoardGameGeekObjectId = 4
                },
                PlayerGameResults = playerGameResults
            };
            playedGames = new List<PlayedGame>
            {
                playedGame
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGames.AsQueryable());

            var results = autoMocker.ClassUnderTest.SearchPlayedGames(new PlayedGameFilter()).First();

            Assert.That(results.PlayedGameId, Is.EqualTo(playedGame.Id));
            Assert.That(results.GameDefinitionName, Is.EqualTo(playedGame.GameDefinition.Name));
            Assert.That(results.GameDefinitionId, Is.EqualTo(playedGame.GameDefinitionId));
            Assert.That(results.DateLastUpdated, Is.EqualTo(playedGame.DateCreated));
            Assert.That(results.DatePlayed, Is.EqualTo(playedGame.DatePlayed));
            Assert.That(results.GamingGroupId, Is.EqualTo(playedGame.GamingGroupId));
            Assert.That(results.GamingGroupName, Is.EqualTo(playedGame.GamingGroup.Name));
            Assert.That(results.PlayerGameResults, Is.SameAs(playedGame.PlayerGameResults));
        }

        [Test]
        public void ItReturnsAnEmptyListIfThereAreNoSearchResults()
        {
            autoMocker = new RhinoAutoMocker<PlayedGameRetriever>();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(new List<PlayedGame>().AsQueryable());
            var results = autoMocker.ClassUnderTest.SearchPlayedGames(new PlayedGameFilter());

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        public void ItFiltersOnStartDateGameLastUpdated()
        {
            var filter = new PlayedGameFilter
            {
                StartDateGameLastUpdated = "2015-04-01"
            };

            var results = autoMocker.ClassUnderTest.SearchPlayedGames(filter);

            Assert.That(results.Count, Is.EqualTo(1));
            Assert.True(results.All(x => x.DateLastUpdated >= new DateTime(2015, 4, 1)));
        }

        [Test]
        public void ItLimitsSearchResultsToTheMaximumSpecified()
        {
            const int MAX_RESULTS = 1;
            var filter = new PlayedGameFilter
            {
                MaximumNumberOfResults = MAX_RESULTS
            };

            var results = autoMocker.ClassUnderTest.SearchPlayedGames(filter);

            Assert.That(results.Count, Is.EqualTo(MAX_RESULTS));
        }
    }
}
