using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
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
    }
}
