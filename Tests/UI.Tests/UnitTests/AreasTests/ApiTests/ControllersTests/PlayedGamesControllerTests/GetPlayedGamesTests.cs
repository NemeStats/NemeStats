using System;
using System.Net;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
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
            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            var actualResponse = autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayedGameSearchResultsMessage>(actualResponse, HttpStatusCode.OK);
            Assert.That(actualData.PlayedGames.Count, Is.EqualTo(0));
        }

        [Test]
        public void ItFiltersOnStartDateGameLastUpdated()
        {
            var filterMessage = new PlayedGameFilterMessage
            {
                StartDateGameLastUpdated = "2015-04-15"
            };

            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.StartDateGameLastUpdated == filterMessage.StartDateGameLastUpdated)));
        }

        [Test]
        public void ItFiltersOnTheGamingGroupId()
        {
            const int EXPECTED_GAMING_GROUP_ID = 1;
            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            autoMocker.ClassUnderTest.GetPlayedGames(new PlayedGameFilterMessage(), EXPECTED_GAMING_GROUP_ID);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.GamingGroupId == EXPECTED_GAMING_GROUP_ID)));
        }

        [Test]
        public void ItFiltersOnPlayerId()
        {
            const int EXPECTED_PLAYER_ID = 1;
            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            autoMocker.ClassUnderTest.GetPlayedGames(new PlayedGameFilterMessage{ PlayerId = EXPECTED_PLAYER_ID }, 1);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.PlayerId == EXPECTED_PLAYER_ID)));
        }

        [Test]
        public void ItFiltersOnGameDefinitionId()
        {
            const int EXPECTED_PLAYER_ID = 1;
            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(new List<PlayedGameSearchResult>());

            autoMocker.ClassUnderTest.GetPlayedGames(new PlayedGameFilterMessage { PlayerId = EXPECTED_PLAYER_ID }, 1);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.PlayerId == EXPECTED_PLAYER_ID)));
        }

        [Test]
        public void ItLimitsTheNumberOfResultsIfSpecified()
        {
            var filterMessage = new PlayedGameFilterMessage
            {
                MaximumNumberOfResults = 100
            };

            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(Arg<PlayedGameFilter>.Is.Anything))
                .Return(new List<PlayedGameSearchResult>());

            autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            autoMocker.Get<IPlayedGameRetriever>().AssertWasCalled(mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Matches(filter => filter.MaximumNumberOfResults == filterMessage.MaximumNumberOfResults)));
        }

        [Test]
        public void ItReturnsTheCorrectPlayedGames()
        {
            var expectedSingleResult = new PlayedGameSearchResult
            {
                BoardGameGeekGameDefinitionId = 1,
                DateLastUpdated = DateTime.UtcNow.Date,
                DatePlayed = DateTime.UtcNow.Date,
                GameDefinitionId = 2,
                GameDefinitionName = "some game definition name",
                GamingGroupId = 3,
                GamingGroupName = "some gaming group name",
                Notes = "some notes",
                PlayedGameId = 4,
                PlayerGameResults = new List<PlayerResult>
                {
                    new PlayerResult
                    {
                        GameRank = 1,
                        PlayerId = 2,
                        PointsScored = 3,
                        NemeStatsPointsAwarded = 4,
                        PlayerName = "some player name"
                    }
                }
            };

            var expectedResults = new List<PlayedGameSearchResult>
            {
                expectedSingleResult
            };
            var filterMessage = new PlayedGameFilterMessage();
            autoMocker.Get<IPlayedGameRetriever>().Expect(
                mock => mock.SearchPlayedGames(
                Arg<PlayedGameFilter>.Is.Anything))
                      .Return(expectedResults);

            var actualResponse = autoMocker.ClassUnderTest.GetPlayedGames(filterMessage, 1);

            var actualData = AssertThatApiAction.ReturnsThisTypeWithThisStatusCode<PlayedGameSearchResultsMessage>(actualResponse, HttpStatusCode.OK);
            Assert.That(actualData.PlayedGames.Count, Is.EqualTo(1));
            var actualSinglePlayedGame = actualData.PlayedGames[0];
            Assert.That(actualSinglePlayedGame.BoardGameGeekGameDefinitionId, Is.EqualTo(expectedSingleResult.BoardGameGeekGameDefinitionId));
            Assert.That(actualSinglePlayedGame.DateLastUpdated, Is.EqualTo(expectedSingleResult.DateLastUpdated.ToString("yyyy-MM-dd")));
            Assert.That(actualSinglePlayedGame.DatePlayed, Is.EqualTo(expectedSingleResult.DatePlayed.ToString("yyyy-MM-dd")));
            Assert.That(actualSinglePlayedGame.GameDefinitionId, Is.EqualTo(expectedSingleResult.GameDefinitionId));
            Assert.That(actualSinglePlayedGame.GameDefinitionName, Is.EqualTo(expectedSingleResult.GameDefinitionName));
            Assert.That(actualSinglePlayedGame.GamingGroupId, Is.EqualTo(expectedSingleResult.GamingGroupId));
            Assert.That(actualSinglePlayedGame.GamingGroupName, Is.EqualTo(expectedSingleResult.GamingGroupName));
            Assert.That(actualSinglePlayedGame.Notes, Is.EqualTo(expectedSingleResult.Notes));
            Assert.That(actualSinglePlayedGame.PlayedGameId, Is.EqualTo(expectedSingleResult.PlayedGameId));
            Assert.That(actualSinglePlayedGame.PlayerGameResults[0].GameRank, Is.EqualTo(expectedSingleResult.PlayerGameResults[0].GameRank));
            Assert.That(actualSinglePlayedGame.PlayerGameResults[0].NemeStatsPointsAwarded, Is.EqualTo(expectedSingleResult.PlayerGameResults[0].NemeStatsPointsAwarded));
            Assert.That(actualSinglePlayedGame.PlayerGameResults[0].PlayerId, Is.EqualTo(expectedSingleResult.PlayerGameResults[0].PlayerId));
            Assert.That(actualSinglePlayedGame.PlayerGameResults[0].PlayerName, Is.EqualTo(expectedSingleResult.PlayerGameResults[0].PlayerName));
            Assert.That(actualSinglePlayedGame.PlayerGameResults[0].PointsScored, Is.EqualTo(expectedSingleResult.PlayerGameResults[0].PointsScored));
        }
    }
}
