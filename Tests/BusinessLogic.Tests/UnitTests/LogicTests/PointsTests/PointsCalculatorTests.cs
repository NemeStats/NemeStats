using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models.Games;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests
{
    [TestFixture]
    public class PointsCalculatorTests
    {
        private const int FIRST_PLACE = 1;
        private const int SECOND_PLACE = 2;
        private const int THIRD_PLACE = 3;

        //TODO ItThrowsAnArgumentExceptionIfAGivenPlayerHasMultipleRanks
        //TODO ItThrowsAnArgumentExceptionIfThereAreMoreThanFifteenPlayers

        [Test]
        public void ItGivesTwoPointsToEachPlayerIfEveryoneLost()
        {
            int losingRank = 2;
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank
            {
                PlayerId = 11,
                GameRank = losingRank
            });
            playerRanks.Add(new PlayerRank
            {
                PlayerId = 132,
                GameRank = losingRank
            });
            playerRanks.Add(new PlayerRank
            {
                PlayerId = 13,
                GameRank = losingRank
            });
            playerRanks.Add(new PlayerRank
            {
                PlayerId = 135,
                GameRank = losingRank
            });

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            Assert.That(actualPointsAwarded.All(x => x.Value == 2));
        }

        [Datapoint]
        private readonly int[] numberOfPlayersArray = new[]
        {
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            20
        };

        [Test]
        [TestCaseSource("numberOfPlayersArray")]
        public void ItGivesAboutTenPointsPerPlayerWhenRanksAreEvenlyDistributed(int numberOfPlayers)
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerRanks.Add(new PlayerRank
                {
                    GameRank = i + 1,
                    PlayerId = i
                });
            }

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            //each player could round up at most 1 integer value
            int minimumPointsAwarded = PointsCalculator.POINTS_PER_PLAYER * numberOfPlayers;
            int maxPointsAwarded = PointsCalculator.POINTS_PER_PLAYER * numberOfPlayers + numberOfPlayers;
            Assert.That(actualPointsAwarded.Sum(x => x.Value), Is.InRange(minimumPointsAwarded, maxPointsAwarded));
        }

        [Test]
        [TestCaseSource("numberOfPlayersArray")]
        public void ItGivesAboutTenPointsPerPlayerWhenRanksClumped(int numberOfPlayers)
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                playerRanks.Add(new PlayerRank
                {
                    GameRank = (i + 1) % 2,
                    PlayerId = i
                });
            }

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            //each player could round up at most 1 integer value
            int maxPointsAwarded = PointsCalculator.POINTS_PER_PLAYER * numberOfPlayers + numberOfPlayers;
            Assert.That(actualPointsAwarded.Sum(x => x.Value), Is.InRange(PointsCalculator.POINTS_PER_PLAYER, maxPointsAwarded));
        }

        [Test]
        public void ItGivesOutTenPointsToEachPlayerWhenEveryoneWins()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();

            for (int i = 0; i < 13; i++)
            {
                playerRanks.Add(new PlayerRank
                {
                    GameRank = FIRST_PLACE,
                    PlayerId = i
                });
            }

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            Assert.That(actualPointsAwarded.All(x => x.Value == 10));
        }

        [Test]
        public void ItWorksForOneWinnerAndOneLoser()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            int winningPlayerId = 123;
            int losingPlayerId = 456;

            playerRanks.Add(new PlayerRank
            {
                GameRank = FIRST_PLACE,
                PlayerId = winningPlayerId
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = SECOND_PLACE,
                PlayerId = losingPlayerId
            });

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            int expectedWinnerPoints = 14;
            int expectedLoserPoints = 7;
            Assert.That(actualPointsAwarded[winningPlayerId], Is.EqualTo(expectedWinnerPoints));
            Assert.That(actualPointsAwarded[losingPlayerId], Is.EqualTo(expectedLoserPoints));
        }

        [Test]
        public void ItWorksForTwoWinnersAndOneLoser()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            int winningPlayerId1 = 123;
            int winningPlayerId2 = 124;
            int losingPlayerId = 456;

            playerRanks.Add(new PlayerRank
            {
                GameRank = FIRST_PLACE,
                PlayerId = winningPlayerId1
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = FIRST_PLACE,
                PlayerId = winningPlayerId2
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = SECOND_PLACE,
                PlayerId = losingPlayerId
            });

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            int expectedWinnerPoints = 13;
            int expectedLoserPoints = 5;
            Assert.That(actualPointsAwarded[winningPlayerId1], Is.EqualTo(expectedWinnerPoints));
            Assert.That(actualPointsAwarded[winningPlayerId2], Is.EqualTo(expectedWinnerPoints));
            Assert.That(actualPointsAwarded[losingPlayerId], Is.EqualTo(expectedLoserPoints));
        }

        [Test]
        public void ItWorksForOneWinnerAndTwoLosers()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            int winningPlayerId1 = 123;
            int losingPlayerId1 = 124;
            int losingPlayerId2 = 456;

            playerRanks.Add(new PlayerRank
            {
                GameRank = FIRST_PLACE,
                PlayerId = winningPlayerId1
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = SECOND_PLACE,
                PlayerId = losingPlayerId1
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = SECOND_PLACE,
                PlayerId = losingPlayerId2
            });

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            int expectedWinnerPoints = 15;
            int expectedLoserPoints = 8;
            Assert.That(actualPointsAwarded[winningPlayerId1], Is.EqualTo(expectedWinnerPoints));
            Assert.That(actualPointsAwarded[losingPlayerId1], Is.EqualTo(expectedLoserPoints));
            Assert.That(actualPointsAwarded[losingPlayerId2], Is.EqualTo(expectedLoserPoints));
        }

        [Test]
        public void ItWorksForTwoWinnersTwoSecondPlayerAndOneLastPlace()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            int winningPlayerId1 = 100;
            int winningPlayerId2 = 101;
            int secondPlacePlayer1 = 200;
            int secondPlacePlayer2 = 201;
            int thirdPlacePlayerId = 300;

            playerRanks.Add(new PlayerRank
            {
                GameRank = FIRST_PLACE,
                PlayerId = winningPlayerId1
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = FIRST_PLACE,
                PlayerId = winningPlayerId2
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = SECOND_PLACE,
                PlayerId = secondPlacePlayer1
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = SECOND_PLACE,
                PlayerId = secondPlacePlayer2
            });
            playerRanks.Add(new PlayerRank
            {
                GameRank = THIRD_PLACE,
                PlayerId = thirdPlacePlayerId
            });

            Dictionary<int, int> actualPointsAwarded = PointsCalculator.CalculatePoints(playerRanks);

            int expectedFirstPlacePoints = 18;
            int expectedSecondPlacePoints = 7;
            int expectedThirdPlacePoints = 3;
            Assert.That(actualPointsAwarded[winningPlayerId1], Is.EqualTo(expectedFirstPlacePoints));
            Assert.That(actualPointsAwarded[winningPlayerId2], Is.EqualTo(expectedFirstPlacePoints));
            Assert.That(actualPointsAwarded[secondPlacePlayer1], Is.EqualTo(expectedSecondPlacePoints));
            Assert.That(actualPointsAwarded[secondPlacePlayer2], Is.EqualTo(expectedSecondPlacePoints));
            Assert.That(actualPointsAwarded[thirdPlacePlayerId], Is.EqualTo(expectedThirdPlacePoints));

        }

        //10 * (numberOfPlayers) * (fibonacciOf(numberOfPlayers + 1 - r) / fibonacciSumFrom(2, numberOfPlayers))
        //still need to make sure that ranked bucket groupings work correctly.
    }
}
