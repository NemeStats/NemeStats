using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayerQuickStatsForUserTests : PlayerRetrieverTestBase
    {
        private readonly string userId = "user id";
        private readonly NemePointsSummary expectedSummary = new NemePointsSummary(1, 3, 5);
        private readonly int expectedPlayerId = 2;
        private Player expectedPlayer;

        [SetUp]
        public void SetUp()
        {
            autoMocker.ClassUnderTest.Expect(mock => mock.GetNemePointsSummary(expectedPlayerId)).Return(expectedSummary);

            expectedPlayer = new Player
            {
                Id = expectedPlayerId,
                ApplicationUserId = userId,
                GamingGroupId = gamingGroupId
            };

            autoMocker.Get<IDataContext>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IDataContext>().Replay();
        }

        [Test]
        public void ItSetsThePlayerIdToNullAndLeavesOtherPlayerStatusEmptyIfThereIsNoPlayerForTheCurrentUser()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());

            var result = autoMocker.ClassUnderTest.GetPlayerQuickStatsForUser(userId, gamingGroupId);

            Assert.That(result.PlayerId, Is.Null);
            Assert.That(result.TotalGamesPlayed, Is.EqualTo(0));
            Assert.That(result.TotalGamesWon, Is.EqualTo(0));
        }

        [Test]
        public void ItReturnsTheTotalNemePointsForTheUser()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>
            {
                expectedPlayer
            }.AsQueryable());
            autoMocker.ClassUnderTest.Expect(mock => mock.GetGameDefinitionTotals(expectedPlayer.Id))
                .Return(new GameDefinitionTotals());
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(
                Arg<int>.Is.Anything, 
                Arg<int>.Is.Anything,
                Arg<IDateRangeFilter>.Is.Anything)).Return(new List<PlayedGame>());

            var result = autoMocker.ClassUnderTest.GetPlayerQuickStatsForUser(userId, gamingGroupId);

            Assert.That(result.NemePointsSummary, Is.EqualTo(expectedSummary));
        }

        [Test]
        public void ItReturnsTheTotalGamesWonAndTotalGamesPlayed()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>
            {
                expectedPlayer
            }.AsQueryable());

            var gameDefinitionTotals = new GameDefinitionTotals();
            autoMocker.ClassUnderTest.Expect(mock => mock.GetGameDefinitionTotals(expectedPlayer.Id))
                .Return(gameDefinitionTotals);
            var topLevelTotals = new PlayerRetriever.TopLevelTotals
            {
                TotalGames = 10,
                TotalGamesWon = 3
            };
            autoMocker.ClassUnderTest.Expect(mock => mock.GetTopLevelTotals(gameDefinitionTotals))
                .Return(topLevelTotals);
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(
                Arg<int>.Is.Anything, 
                Arg<int>.Is.Anything,
                Arg<IDateRangeFilter>.Is.Anything)).Return(new List<PlayedGame>());

            var result = autoMocker.ClassUnderTest.GetPlayerQuickStatsForUser(userId, gamingGroupId);

            Assert.That(result.TotalGamesWon, Is.EqualTo(topLevelTotals.TotalGamesWon));
            Assert.That(result.TotalGamesPlayed, Is.EqualTo(topLevelTotals.TotalGames));
        }

        [Test]
        public void ItReturnsTheLastGamingGroupGamePlayed()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>
            {
                expectedPlayer
            }.AsQueryable());
            autoMocker.ClassUnderTest.Expect(mock => mock.GetGameDefinitionTotals(expectedPlayer.Id))
                .Return(new GameDefinitionTotals());
            var expectedBggGameDefinitionId = 2;

            var expectedPlayedGame = MockRepository.GeneratePartialMock<PlayedGame>();
            expectedPlayedGame.DatePlayed = new DateTime();
            expectedPlayedGame.GameDefinitionId = 1;
            expectedPlayedGame.WinnerType = WinnerTypes.PlayerWin;
            expectedPlayedGame.GameDefinition = new GameDefinition
            {
                Name = "some game definition name",
                BoardGameGeekGameDefinitionId = expectedBggGameDefinitionId,
                BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
                {
                    Name = "bgg name",
                    Thumbnail = "some thumbnail",
                    Id = expectedBggGameDefinitionId
                }
            };
            var playedGames = new List<PlayedGame>
            {
                expectedPlayedGame
            };
            var expectedWinningPlayer = new Player
            {
                Id = 93,
                Name = "some winning player name"
            };
            expectedPlayedGame.Expect(mock => mock.WinningPlayer).Repeat.Any().Return(expectedWinningPlayer);
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(1, expectedPlayer.GamingGroupId))
                .Return(playedGames);

            var result = autoMocker.ClassUnderTest.GetPlayerQuickStatsForUser(userId, gamingGroupId);

            var lastGamingGroupGame = result.LastGamingGroupGame;
            Assert.That(lastGamingGroupGame, Is.Not.Null);
            Assert.That(lastGamingGroupGame.DatePlayed, Is.EqualTo(expectedPlayedGame.DatePlayed));
            Assert.That(lastGamingGroupGame.GameDefinitionId, Is.EqualTo(expectedPlayedGame.GameDefinitionId));
            Assert.That(lastGamingGroupGame.GameDefinitionName, Is.EqualTo(expectedPlayedGame.GameDefinition.Name));
            Assert.That(lastGamingGroupGame.PlayedGameId, Is.EqualTo(expectedPlayedGame.Id));
            var expectedBggUri = BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(expectedBggGameDefinitionId);
            Assert.That(lastGamingGroupGame.BoardGameGeekUri, Is.EqualTo(expectedBggUri));
            Assert.That(lastGamingGroupGame.ThumbnailImageUrl, Is.EqualTo(expectedPlayedGame.GameDefinition.BoardGameGeekGameDefinition.Thumbnail));
            Assert.That(lastGamingGroupGame.WinnerType, Is.EqualTo(expectedPlayedGame.WinnerType));
            Assert.That(lastGamingGroupGame.WinningPlayerName, Is.EqualTo(expectedWinningPlayer.Name));
            Assert.That(lastGamingGroupGame.WinningPlayerId, Is.EqualTo(expectedWinningPlayer.Id));
        }

        [Test]
        public void ItLeavesTheLastGamingGroupGameNullIfThereAreNoGamesFromThatGamingGroup()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());

            var playedGames = new List<PlayedGame>();
           
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(1, expectedPlayer.GamingGroupId))
                .Return(playedGames);

            var result = autoMocker.ClassUnderTest.GetPlayerQuickStatsForUser(userId, gamingGroupId);

            Assert.That(result.LastGamingGroupGame, Is.Null);
        }
    }
}
