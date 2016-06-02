using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.PlayedGame;
using UI.Models.Points;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class SearchHttpPostTests : PlayedGameControllerTestBase
    {
        private const int GAME_DEFINITION_B_ID = 1;
        private const string GAME_DEFINITION_NAME_B = "B - some game definition name";
        private const int GAME_DEFINITION_A_ID = 2;
        private const string GAME_DEFINITION_NAME_A = "A - some game definition name";

        [OneTimeSetUpAttribute]
        public void TestFixtureSetup()
        {
            AutomapperConfiguration.Configure();
        }

        [SetUp]
        public void SetUp()
        {
            var gameDefinitionNames = new List<GameDefinitionName>
            {
                new GameDefinitionName
                {
                    Name = GAME_DEFINITION_NAME_B,
                    Id = GAME_DEFINITION_B_ID
                },
                new GameDefinitionName
                {
                    Name = GAME_DEFINITION_NAME_A,
                    Id = GAME_DEFINITION_A_ID
                }
            };
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(this.currentUser.CurrentGamingGroupId)).Return(gameDefinitionNames);
        }

        [Test]
        public void ItReturnsTheCorrectView()
        {
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(Arg<PlayedGameFilter>.Is.Anything)).Return(new List<PlayedGameSearchResult>());

            var actualResults = autoMocker.ClassUnderTest.Search(new PlayedGamesFilterViewModel(), currentUser) as ViewResult;

            Assert.That(actualResults.ViewName, Is.EqualTo(MVC.PlayedGame.Views.Search));
        }

        [Test]
        public void ItReturnsTheCorrectViewModelType()
        {
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(Arg<PlayedGameFilter>.Is.Anything)).Return(new List<PlayedGameSearchResult>());

            var actualResults = autoMocker.ClassUnderTest.Search(new PlayedGamesFilterViewModel(), currentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel, Is.TypeOf(typeof(SearchViewModel)));
        }

        [Test]
        public void ItReturnsTheCorrectFilterViewModel()
        {
            
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(null))
                .IgnoreArguments()
                .Return(new List<PlayedGameSearchResult>());
            var filter = new PlayedGamesFilterViewModel
            {
                DatePlayedStart = new DateTime(2015, 1, 1),
                DatePlayedEnd = new DateTime(2015, 2, 2),
                GameDefinitionId = 1
            };

            var actualResults = autoMocker.ClassUnderTest.Search(filter, currentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel, Is.TypeOf(typeof(SearchViewModel)));
            Assert.That(actualViewModel.Filter.DatePlayedEnd, Is.EqualTo(filter.DatePlayedEnd));
            Assert.That(actualViewModel.Filter.DatePlayedStart, Is.EqualTo(filter.DatePlayedStart));
            Assert.That(actualViewModel.Filter.GameDefinitionId, Is.EqualTo(filter.GameDefinitionId));
        }

        [Test]
        public void ItReturnsTheCorrectSearchResults()
        {
            var filter = new PlayedGamesFilterViewModel
            {
                DatePlayedEnd = DateTime.Now.Date,
                DatePlayedStart = DateTime.Now.Date.AddDays(-1),
                GameDefinitionId = 1
            };
            var expectedSearchResults = new List<PlayedGameSearchResult>
            {
                new PlayedGameSearchResult
                {
                    BoardGameGeekGameDefinitionId = 1,
                    GameDefinitionId = 2,
                    GameDefinitionName = "some game definition name",
                    DatePlayed = new DateTime().Date,
                    PlayedGameId = 3,
                    PlayerGameResults = new List<PlayerResult>
                    {
                        new PlayerResult
                        {
                            GameRank = 1,
                            NemeStatsPointsAwarded = 3,
                            GameDurationBonusNemePoints = 1,
                            GameWeightBonusNemePoints = 4,
                            PlayerId = 4,
                            PlayerName = "some player name",
                            PointsScored = 5
                        }
                    },
                    WinnerType = WinnerTypes.TeamWin
                }
            };

            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(
                    x => x.GamingGroupId == currentUser.CurrentGamingGroupId
                        && x.GameDefinitionId == filter.GameDefinitionId
                        && x.StartDateGameLastUpdated ==
                        filter.DatePlayedStart.Value.ToString("yyyy-MM-dd")
                        && x.EndDateGameLastUpdated ==
                        filter.DatePlayedEnd.Value.ToString("yyyy-MM-dd"))))
                          .Return(expectedSearchResults);

            var actualResults = autoMocker.ClassUnderTest.Search(filter, currentUser) as ViewResult;

            var actualPlayedGameSearchResult = ((SearchViewModel)actualResults.Model).PlayedGames.PlayedGameDetailsViewModels[0];
            var expectedPlayedGameSearchResult = expectedSearchResults[0];
            Assert.That(actualPlayedGameSearchResult.DatePlayed, Is.EqualTo(expectedPlayedGameSearchResult.DatePlayed));
            Assert.That(actualPlayedGameSearchResult.GameDefinitionId, Is.EqualTo(expectedPlayedGameSearchResult.GameDefinitionId));
            Assert.That(actualPlayedGameSearchResult.GameDefinitionName, Is.EqualTo(expectedPlayedGameSearchResult.GameDefinitionName));
            Assert.That(actualPlayedGameSearchResult.GamingGroupId, Is.EqualTo(expectedPlayedGameSearchResult.GamingGroupId));
            Assert.That(actualPlayedGameSearchResult.GamingGroupName, Is.EqualTo(expectedPlayedGameSearchResult.GamingGroupName));
            Assert.That(actualPlayedGameSearchResult.Notes, Is.EqualTo(expectedPlayedGameSearchResult.Notes));
            Assert.That(actualPlayedGameSearchResult.WinnerType, Is.EqualTo(expectedPlayedGameSearchResult.WinnerType));
            var actualPlayerResult = actualPlayedGameSearchResult.PlayerResults[0];
            var expectedPlayerResult = expectedPlayedGameSearchResult.PlayerGameResults[0];
            Assert.That(actualPlayerResult.DatePlayed, Is.EqualTo(expectedPlayedGameSearchResult.DatePlayed));
            Assert.That(actualPlayerResult.GameDefinitionId, Is.EqualTo(expectedPlayedGameSearchResult.GameDefinitionId));
            Assert.That(actualPlayerResult.GameDefinitionName, Is.EqualTo(expectedPlayedGameSearchResult.GameDefinitionName));
            Assert.That(actualPlayerResult.GameRank, Is.EqualTo(expectedPlayerResult.GameRank));
            var expectedNemePointsSummary = new NemePointsSummaryViewModel(expectedPlayerResult.NemeStatsPointsAwarded, expectedPlayerResult.GameDurationBonusNemePoints, expectedPlayerResult.GameWeightBonusNemePoints);
            Assert.That(actualPlayerResult.NemePointsSummary, Is.EqualTo(expectedNemePointsSummary));
            Assert.That(actualPlayerResult.PlayedGameId, Is.EqualTo(expectedPlayedGameSearchResult.PlayedGameId));
            Assert.That(actualPlayerResult.PlayerId, Is.EqualTo(expectedPlayerResult.PlayerId));
            Assert.That(actualPlayerResult.PlayerName, Is.EqualTo(expectedPlayerResult.PlayerName));
            Assert.That(actualPlayerResult.WinnerType, Is.EqualTo(expectedPlayedGameSearchResult.WinnerType));
        }

        [Test]
        public void ItDoesNotShowTheSearchLinkOnPlayedGameSearchResults()
        {
            autoMocker.Get<IPlayedGameRetriever>()
                .Expect(mock => mock.SearchPlayedGames(Arg<PlayedGameFilter>.Is.Anything))
                .Return(new List<PlayedGameSearchResult>());

            var actualResults = autoMocker.ClassUnderTest.Search(new PlayedGamesFilterViewModel(), currentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel.PlayedGames.ShowSearchLinkInResultsHeader, Is.False);
        }

    }
}
