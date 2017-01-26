using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class MakePlayerGameResultsTests : PlayedGameSaverTestBase
    {
        [Test]
        public void ItSetsNemePointsForEachPlayerGameResult()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerOneGameRank = 1;
            var playerTwoGameRank = 2;
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = playerOneId,
                    GameRank = playerOneGameRank
                },
                new PlayerRank
                {
                    PlayerId = playerTwoId,
                    GameRank = playerTwoGameRank
                }
            };
            var newlyCompletedPlayedGame = new NewlyCompletedGame
            {
                GameDefinitionId = GameDefinition.Id,
                PlayerRanks = playerRanks
            };

            var playerOneScorecard = new PointsScorecard
            {
                BasePoints = 1,
                GameDurationBonusPoints = 2,
                GameWeightBonusPoints = 3
            };
            var playerTwoScorecard = new PointsScorecard
            {
                BasePoints = 4,
                GameDurationBonusPoints = 5,
                GameWeightBonusPoints = 6
            };
            var expectedPointsDictionary = new Dictionary<int, PointsScorecard>
            {
                {playerOneId, playerOneScorecard},
                {playerTwoId, playerTwoScorecard}
            };

            AutoMocker.Get<IPointsCalculator>().Expect(mock => mock.CalculatePoints(playerRanks, null))
                .Return(expectedPointsDictionary);

            //--Act
            var playerGameResults = AutoMocker.ClassUnderTest.MakePlayerGameResults(newlyCompletedPlayedGame, null, DataContextMock);

            var playerOne = playerGameResults.First(gameResult => gameResult.PlayerId == playerOneId);
            Assert.That(playerOne.GameDurationBonusPoints, Is.EqualTo(playerOneScorecard.GameDurationBonusPoints));
            Assert.That(playerOne.GameWeightBonusPoints, Is.EqualTo(playerOneScorecard.GameWeightBonusPoints));
            Assert.That(playerOne.NemeStatsPointsAwarded, Is.EqualTo(playerOneScorecard.BasePoints));

            var playerTwo = playerGameResults.First(gameResult => gameResult.PlayerId == playerTwoId);
            Assert.That(playerTwo.GameDurationBonusPoints, Is.EqualTo(playerTwoScorecard.GameDurationBonusPoints));
            Assert.That(playerTwo.GameWeightBonusPoints, Is.EqualTo(playerTwoScorecard.GameWeightBonusPoints));
            Assert.That(playerTwo.NemeStatsPointsAwarded, Is.EqualTo(playerTwoScorecard.BasePoints));
        }

    }
}
