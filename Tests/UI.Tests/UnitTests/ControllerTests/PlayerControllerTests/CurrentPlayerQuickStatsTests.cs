using BusinessLogic.Logic.Players;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System;
using System.Web.Mvc;
using BusinessLogic.Models.Points;
using UI.Models.Players;
using UI.Models.Points;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class CurrentPlayerQuickStatsTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsTheCurrentUsersPlayerQuickStatsViewModel()
        {
            var expectedPlayerQuickSummary = new PlayerQuickStats
            {
                PlayerId = 1,
                TotalGamesPlayed = 4,
                TotalGamesWon = 3,
                NemePointsSummary = new NemePointsSummary(1, 3, 5),
                LastGamingGroupGame = new PlayedGameQuickStats
                {
                    BoardGameGeekUri = new Uri("http://a.com"),
                    DatePlayed = new DateTime(),
                    GameDefinitionId = 10,
                    GameDefinitionName = "some game definition name",
                    PlayedGameId = 12,
                    ThumbnailImageUrl = "some url",
                    WinnerType = WinnerTypes.PlayerWin,
                    WinningPlayerId = 13,
                    WinningPlayerName = "some winning player name"
                }
            };

            autoMocker.Get<IPlayerRetriever>()
                .Expect(mock => mock.GetPlayerQuickStatsForUser(currentUser.Id, currentUser.CurrentGamingGroupId))
                .Return(expectedPlayerQuickSummary);

            var result = autoMocker.ClassUnderTest.CurrentPlayerQuickStats(currentUser) as ViewResult;

            Assert.That(result, Is.Not.Null);
            var actualModel = result.Model as PlayerQuickStatsViewModel;
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.PlayerId, Is.EqualTo(expectedPlayerQuickSummary.PlayerId));
            var expectedNemePointsSummary = new NemePointsSummaryViewModel(
                expectedPlayerQuickSummary.NemePointsSummary.BaseNemePoints,
                expectedPlayerQuickSummary.NemePointsSummary.GameDurationBonusNemePoints,
                expectedPlayerQuickSummary.NemePointsSummary.WeightBonusNemePoints);
            Assert.That(actualModel.NemePointsSummary, Is.EqualTo(expectedNemePointsSummary));
            Assert.That(actualModel.TotalGamesWon, Is.EqualTo(expectedPlayerQuickSummary.TotalGamesWon));
            Assert.That(actualModel.TotalGamesPlayed, Is.EqualTo(expectedPlayerQuickSummary.TotalGamesPlayed));
            Assert.That(actualModel.LastGamingGroupGame, Is.Not.Null);
            var lastGamingGroupGameViewModel = actualModel.LastGamingGroupGame;
            var playedGameQuickStats = expectedPlayerQuickSummary.LastGamingGroupGame;
            Assert.That(lastGamingGroupGameViewModel.BoardGameGeekUri, Is.EqualTo(playedGameQuickStats.BoardGameGeekUri));
            Assert.That(lastGamingGroupGameViewModel.DatePlayed, Is.EqualTo(playedGameQuickStats.DatePlayed));
            Assert.That(lastGamingGroupGameViewModel.GameDefinitionName, Is.EqualTo(playedGameQuickStats.GameDefinitionName));
            Assert.That(lastGamingGroupGameViewModel.PlayedGameId, Is.EqualTo(playedGameQuickStats.PlayedGameId));
            Assert.That(lastGamingGroupGameViewModel.ThumbnailImageUrl, Is.EqualTo(playedGameQuickStats.ThumbnailImageUrl));
            Assert.That(lastGamingGroupGameViewModel.WinnerType, Is.EqualTo(playedGameQuickStats.WinnerType));
            Assert.That(lastGamingGroupGameViewModel.WinningPlayerId, Is.EqualTo(playedGameQuickStats.WinningPlayerId));
            Assert.That(lastGamingGroupGameViewModel.WinningPlayerName, Is.EqualTo(playedGameQuickStats.WinningPlayerName));
        }

        [Test]
        public void ItLeavesTheLastGamingGroupGameNullIfThereAreNoGames()
        {
            autoMocker.Get<IPlayerRetriever>()
               .Expect(mock => mock.GetPlayerQuickStatsForUser(currentUser.Id, currentUser.CurrentGamingGroupId))
               .Return(new PlayerQuickStats());

            var result = autoMocker.ClassUnderTest.CurrentPlayerQuickStats(currentUser) as ViewResult;

            var actualModel = result.Model as PlayerQuickStatsViewModel;
            Assert.That(actualModel.LastGamingGroupGame, Is.Null);
        }
    }
}