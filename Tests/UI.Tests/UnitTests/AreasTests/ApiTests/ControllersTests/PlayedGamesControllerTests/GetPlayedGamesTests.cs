using System.Net;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net.Http;
using Shouldly;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class GetPlayedGamesTests : ApiControllerTestBase<PlayedGamesController>
    {
        [Test]
        public void ItReturnsAnEmptyListIfThereAreNoResults()
        {
            var filterMessage = new PlayedGameFilterMessage();
            _autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            var actualResponse = _autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayedGameSearchResultsMessage>(actualResponse, HttpStatusCode.OK);
            Assert.That(actualData.PlayedGames.Count, Is.EqualTo(0));
        }

        [Test]
        public void ItAlwaysUsesTheSpecifiedGamingGroupIdAndNotTheOneOnTheMessage()
        {
            var filterMessage = new PlayedGameFilterMessage();

            var responseMessage = new HttpResponseMessage();
            _autoMocker.PartialMockTheClassUnderTest();
            _autoMocker.ClassUnderTest.Expect(partialMock => partialMock.GetPlayedGameSearchResults(null))
                .IgnoreArguments()
                .Return(responseMessage);
            var expectedGamingGroupId = 1;

            _autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, expectedGamingGroupId);

            _autoMocker.ClassUnderTest.AssertWasCalled(partialMock => partialMock.GetPlayedGameSearchResults(
                Arg<PlayedGameFilterMessage>.Matches(x => x.GamingGroupId == expectedGamingGroupId)));
        }

        [Test]
        public void ItReturnsTheFilteredPlayedGames()
        {
            var filterMessage = new PlayedGameFilterMessage();

            var responseMessage = new HttpResponseMessage();
            _autoMocker.PartialMockTheClassUnderTest();
            _autoMocker.ClassUnderTest.Expect(partialMock => partialMock.GetPlayedGameSearchResults(filterMessage))
                .Return(responseMessage);

            var actualResponse = _autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            actualResponse.ShouldBeSameAs(responseMessage);
        }
    }
}
