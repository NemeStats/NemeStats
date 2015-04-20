using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net.Http;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class GetPlayedGamesTests : PlayedGamesControllerTestBase
    {
        [Test]
        public void ItReturnsAnEmptyListIfThereAreNoResults()
        {
            var filterMessage = new PlayedGameFilterMessage();
            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            var actualResponse = autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<List<PlayedGameSearchResultMessage>>)));
            ObjectContent<List<PlayedGameSearchResultMessage>> content = actualResponse.Content as ObjectContent<List<PlayedGameSearchResultMessage>>;
            var searchResults = content.Value as List<PlayedGameSearchResultMessage>;
            Assert.That(searchResults.Count, Is.EqualTo(0));
        }

        [Test]
        public void ItFiltersOnStartDateGameLastUpdated()
        {
            var filterMessage = new PlayedGameFilterMessage
            {
                StartDateGameLastUpdated = "2015-04-15"
            };

            autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.StartDateGameLastUpdated == filterMessage.StartDateGameLastUpdated)));
        }

        [Test]
        public void ItLimitsTheNumberOfResultsIfSpecified()
        {
            var filterMessage = new PlayedGameFilterMessage
            {
                MaximumNumberOfResults = 100
            };

            autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.MaximumNumberOfResults == filterMessage.MaximumNumberOfResults)));
        }
    }
}
