using BusinessLogic.Models.Points;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests.GordonPointsTests
{
    [TestFixture]
    public class CalculateGordonPointsTests
    {
        [Test]
        public void ItAwardsPointsEqualToTheNumberOfPlayersToFirstPlace()
        {
            int numberOfPlayers = 3;

            int points = GordonPoints.CalculateGordonPoints(numberOfPlayers, 1);

            Assert.AreEqual(numberOfPlayers, points);
        }

        [Test]
        public void ItAwardsOnePointToTheLastPlacePlayer()
        {
            int points = GordonPoints.CalculateGordonPoints(10, 10);

            Assert.AreEqual(1, points);
        }
    }
}
