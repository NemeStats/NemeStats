using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class SearchHttpPostTests : PlayedGameControllerTestBase
    {
        [Test]
        public void ItReturnsTheCorrectView()
        {
            var actualResults = autoMocker.ClassUnderTest.Search(new PlayedGamesFilterViewModel(), currentUser) as ViewResult;

            Assert.That(actualResults.ViewName, Is.EqualTo(MVC.PlayedGame.Views.Search));
        }

        [Test]
        public void ItReturnsTheCorrectViewModelType()
        {
            var actualResults = autoMocker.ClassUnderTest.Search(new PlayedGamesFilterViewModel(), currentUser) as ViewResult;

            var actualViewModel = actualResults.ViewData.Model as SearchViewModel;
            Assert.That(actualViewModel, Is.TypeOf(typeof(SearchViewModel)));
        }

        [Test]
        public void ItReturnsTheCorrectFilterViewModel()
        {
            var expectedSearchResults = new List<PlayedGameSearchResult>
            {
                new PlayedGameSearchResult
                {
                    BoardGameGeekObjectId = 1,
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
                            PlayerId = 4,
                            PlayerName = "some player name",
                            PointsScored = 5
                        }
                    }
                }
            };
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(null))
                .IgnoreArguments()
                .Return(expectedSearchResults);
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
            var expectedResults = new List<PlayedGameSearchResult>
            {
                new PlayedGameSearchResult
                {
                    
                }
            };
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(
                                                                                         Arg<PlayedGameFilter>.Matches(x => x.GamingGroupId == filter.GameDefinitionId
                                                                                                                            &&
                                                                                                                            x.StartDateGameLastUpdated ==
                                                                                                                            filter.DatePlayedStart.Value.ToString("yyyyMMdd")
                                                                                                                            &&
                                                                                                                            x.EndDateGameLastUpdated ==
                                                                                                                            filter.DatePlayedEnd.Value.ToString("yyyyMMdd"))))
                      .Return(expectedResults);

            var actualResults = autoMocker.ClassUnderTest.Search(filter, currentUser) as ViewResult;
        }


    }
}
