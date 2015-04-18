using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using BusinessLogic.Models.PlayedGames;

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
                Arg<PlayedGameFilter>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            var actualResponse = autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<List<PlayedGameSearchResult>>)));
            ObjectContent<List<PlayedGameSearchResult>> content = actualResponse.Content as ObjectContent<List<PlayedGameSearchResult>>;
            var searchResults = content.Value as List<PlayedGameSearchResult>;
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
                Arg<PlayedGameFilter>.Matches(filter => filter.StartDateGameLastUpdated == filterMessage.StartDateGameLastUpdated),
                Arg<ApplicationUser>.Is.Anything));
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
                Arg<PlayedGameFilter>.Matches(filter => filter.MaximumNumberOfResults == filterMessage.MaximumNumberOfResults),
                Arg<ApplicationUser>.Is.Anything));
        }
    }
}
