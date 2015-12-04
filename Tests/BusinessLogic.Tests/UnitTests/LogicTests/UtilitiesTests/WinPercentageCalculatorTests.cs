using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Utilities;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UtilitiesTests
{
    [TestFixture]
    public class WinPercentageCalculatorTests
    {
        [Test]
        public void ItReturnsZeroWhenThereAreZeroGamesPlayed()
        {
            var result = WinPercentageCalculator.CalculateWinPercentage(0, 0);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void ItReturnsOneHundredPercentIfThereAreOnlyWins()
        {
            var result = WinPercentageCalculator.CalculateWinPercentage(42, 0);

            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void ItReturnsZeroIfThereAreOnlyLosses()
        {
            var result = WinPercentageCalculator.CalculateWinPercentage(0, 21);

            Assert.That(result, Is.EqualTo(0)); 
        }

        [TestCase(1, 1, ExpectedResult = 50)]
        [TestCase(1, 2, ExpectedResult = 33)]
        [TestCase(4, 1, ExpectedResult = 80)]
        public int TestOtherCases(int numberOfWins, int numberOfLosses)
        {
            return WinPercentageCalculator.CalculateWinPercentage(numberOfWins, numberOfLosses); 
        }
    }
}
