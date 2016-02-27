using System.Linq;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests
{
    [TestFixture]
    public class WeightTierCalculatorTests
    {
        private WeightTierCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new WeightTierCalculator();
        }

        [Test]
        public void ItReturnsUnknownWeightIfTheBoardGameGeekWeightIsNull()
        {
            //--act
            var result = _calculator.GetWeightTier(null);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Unknown));
        }

        [Test]
        public void ItReturnsUnknownWeightIfTheBoardGameGeekWeightIsZero()
        {
            //--act
            var result = _calculator.GetWeightTier(0);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Unknown));
        }

        [Test]
        public void ItReturnsCasualWeightIfTheBoardGameGeekWeightIsLessThan1Point8()
        {
            //--arrange
            var weight = new decimal(1.7);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Casual));
        }

        [Test]
        public void ItReturnsEasyWeightIfTheBoardGameGeekWeightIs1Point8()
        {
            //--arrange
            var weight = new decimal(1.8);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Easy));
        }

        [Test]
        public void ItReturnsEasyWeightIfTheBoardGameGeekWeightIsLessThan2Point4()
        {
            //--arrange
            var weight = new decimal(2.3);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Easy));
        }

        [Test]
        public void ItReturnsAdvancedWeightIfTheBoardGameGeekWeightIs2Point4()
        {
            //--arrange
            var weight = new decimal(2.4);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Advanced));
        }

        [Test]
        public void ItReturnsAdvancedWeightIfTheBoardGameGeekWeightIsLessThan3Point3()
        {
            //--arrange
            var weight = new decimal(3.2);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Advanced));
        }

        [Test]
        public void ItReturnsChallengingWeightIfTheBoardGameGeekWeightIs3Point3()
        {
            //--arrange
            var weight = new decimal(3.3);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Challenging));
        }

        [Test]
        public void ItReturnsChallengingWeightIfTheBoardGameGeekWeightIsLessThan4Point1()
        {
            //--arrange
            var weight = new decimal(4.0);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Challenging));
        }

        [Test]
        public void ItReturnsHardcoreWeightIfTheBoardGameGeekWeightIs4Point1()
        {
            //--arrange
            var weight = new decimal(4.1);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Hardcore));
        }

        [Test]
        public void ItReturnsChallengingWeightIfTheBoardGameGeekWeightIsGreaterThanTo4Point1()
        {
            //--arrange
            var weight = new decimal(3566721);

            //--act
            var result = _calculator.GetWeightTier(weight);

            //--assert
            Assert.That(result, Is.EqualTo(WeightTierEnum.Hardcore));
        }
    }
}
