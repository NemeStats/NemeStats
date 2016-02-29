using BusinessLogic.Logic.Points;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests
{
    public class AverageGameDurationTierCalculatorTests
    {
        private AverageGameDurationTierCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new AverageGameDurationTierCalculator();
        }

        [Test]
        public void ItReturnsUnknownIfTheBoardGameGeekPlayingTimeIsNull()
        {
            //--act
            var result = _calculator.GetAverageGameDurationTier(null);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Unknown));
        }

        [Test]
        public void ItReturnsUnknownIfTheBoardGameGeekPlayingTimeIsZero()
        {
            //--act
            var result = _calculator.GetAverageGameDurationTier(0);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Unknown));
        }

        [Test]
        public void ItReturnsVeryShortIfTheBoardGameGeekPlayingTimeIsLessThan30()
        {
            //--arrange
            var time = 29;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.VeryShort));
        }

        [Test]
        public void ItReturnsShortIfTheBoardGameGeekPlayingTimeIs30()
        {
            //--arrange
            var time = 30;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Short));
        }

        [Test]
        public void ItReturnsShortIfTheBoardGameGeekPlayingTimeIsLessThan60()
        {
            //--arrange
            var time = 59;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Short));
        }

        [Test]
        public void ItReturnsMediumIfTheBoardGameGeekPlayingTimeIs60()
        {
            //--arrange
            var time = 60;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Medium));
        }

        [Test]
        public void ItReturnsMediumIfTheBoardGameGeekPlayingTimeIsLessThan120()
        {
            //--arrange
            var time = 119;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Medium));
        }

        [Test]
        public void ItReturnsLongIfTheBoardGameGeekPlayingTimeIs120()
        {
            //--arrange
            var time = 120;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Long));
        }

        [Test]
        public void ItReturnsLongIfTheBoardGameGeekPlayingTimeIsLessThan200()
        {
            //--arrange
            var time = 199;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.Long));
        }

        [Test]
        public void ItReturnsVeryLongIfTheBoardGameGeekPlayingTimeIs200()
        {
            //--arrange
            var time = 200;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.VeryLong));
        }

        [Test]
        public void ItReturnsVeryLongIfTheBoardGameGeekPlayingTimeIsLessThan300()
        {
            //--arrange
            var time = 299;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.VeryLong));
        }

        [Test]
        public void ItReturnsRidiculouslyLongIfTheBoardGameGeekPlayingTimeIs300()
        {
            //--arrange
            var time = 300;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.RidiculouslyLong));
        }

        [Test]
        public void ItReturnsRidiculouslyLongIfTheBoardGameGeekPlayingTimeIsGreaterThan300()
        {
            //--arrange
            var time = 9999;

            //--act
            var result = _calculator.GetAverageGameDurationTier(time);

            //--assert
            Assert.That(result, Is.EqualTo(AverageGameDurationTierEnum.RidiculouslyLong));
        }
    }
}
