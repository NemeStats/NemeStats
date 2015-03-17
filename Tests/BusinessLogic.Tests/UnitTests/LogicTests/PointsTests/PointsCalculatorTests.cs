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

        //10 * (numberOfPlayers) * (fibonacciOf(numberOfPlayers + 1 - r) / fibonacciSumFrom(2, numberOfPlayers))
        //still need to make sure that ranked bucket groupings work correctly.
    }
}
