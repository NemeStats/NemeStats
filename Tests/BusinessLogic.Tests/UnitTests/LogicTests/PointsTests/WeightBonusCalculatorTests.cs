using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests
{
    [TestFixture]
    public class WeightBonusCalculatorTests
    {
        private RhinoAutoMocker<WeightBonusCalculator> _autoMocker;
        private Dictionary<int, PointsScorecard> _startingPointsScorecard;
        private readonly int _playerOneId = 1;
        private readonly int _playerOneBasePoints = 14;
        private readonly int _playerTwoId = 2;
        private readonly int _playerTwoBasePoints = 7;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<WeightBonusCalculator>();
            _startingPointsScorecard = new Dictionary<int, PointsScorecard>
            {
                {
                    _playerOneId,
                    new PointsScorecard
                    {
                        BasePoints = _playerOneBasePoints
                    }
                },
                {
                    _playerTwoId,
                    new PointsScorecard
                    {
                        BasePoints = _playerTwoBasePoints
                    }
                }
            };
        }

        [Test]
        public void ItAwardsNoAdditionalPointsWhenTheWeightTierIsUnknown()
        {
            //--arrange
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(null)).Return(WeightTierEnum.Unknown);

            //--act
            _autoMocker.ClassUnderTest.CalculateWeightBonus(_startingPointsScorecard, null);

            //--assert
            AssertNoBonusPointsAwarded(_startingPointsScorecard);
        }


        [Test]
        public void ItAwardsNoAdditionalPointsForCasualGames()
        {
            //--arrange
            var weight = -1;
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(weight)).Return(WeightTierEnum.Casual);

            //--act
            _autoMocker.ClassUnderTest.CalculateWeightBonus(_startingPointsScorecard, weight);

            //--assert
            AssertNoBonusPointsAwarded(_startingPointsScorecard);
        }

        [Test]
        public void ItAwardsNoAdditionalPointsForEasyGames()
        {
            //--arrange
            var weight = -1;
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(weight)).Return(WeightTierEnum.Easy);
            //--act
            _autoMocker.ClassUnderTest.CalculateWeightBonus(_startingPointsScorecard, weight);

            //--assert
            AssertNoBonusPointsAwarded(_startingPointsScorecard);
        }

        private void AssertNoBonusPointsAwarded(Dictionary<int, PointsScorecard> actualPointsAwarded)
        {
            var playerScorcard = actualPointsAwarded[_playerOneId];
            Assert.That(playerScorcard.BasePoints, Is.EqualTo(_playerOneBasePoints));
            Assert.That(playerScorcard.GameWeightBonusPoints, Is.EqualTo(0));
            Assert.That(playerScorcard.GameDurationBonusPoints, Is.EqualTo(0));

            playerScorcard = actualPointsAwarded[_playerTwoId];
            Assert.That(playerScorcard.BasePoints, Is.EqualTo(_playerTwoBasePoints));
            Assert.That(playerScorcard.GameWeightBonusPoints, Is.EqualTo(0));
            Assert.That(playerScorcard.GameDurationBonusPoints, Is.EqualTo(0));
        }

        [Test]
        public void ItAwardsA20PercentBonusForAdvancedGames()
        {
            //--arrange
            var weight = -1;
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(weight)).Return(WeightTierEnum.Advanced);

            //--act
            _autoMocker.ClassUnderTest.CalculateWeightBonus(_startingPointsScorecard, weight);

            //--assert
            AssertPercentageBonusApplied(_startingPointsScorecard, .2M);
        }

        [Test]
        public void ItAwardsA20PercentBonusForChallengingGames()
        {
            //--arrange
            var weight = -1;
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(weight)).Return(WeightTierEnum.Challenging);

            //--act
            _autoMocker.ClassUnderTest.CalculateWeightBonus(_startingPointsScorecard, weight);

            //--assert
            AssertPercentageBonusApplied(_startingPointsScorecard, .2M);
        }

        [Test]
        public void ItAwardsA30PercentBonusForHardcoreGames()
        {
            //--arrange
            var weight = -1;
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(weight)).Return(WeightTierEnum.Hardcore);

            //--act
            _autoMocker.ClassUnderTest.CalculateWeightBonus(_startingPointsScorecard, weight);

            //--assert
            AssertPercentageBonusApplied(_startingPointsScorecard, .3M);
        }

        private void AssertPercentageBonusApplied(Dictionary<int, PointsScorecard> actualPointsAwarded, decimal expectedMultiplier)
        {
            var playerScorecard = actualPointsAwarded[_playerOneId];
            Assert.That(playerScorecard.BasePoints, Is.EqualTo(_playerOneBasePoints));
            var expectedPoints = Math.Ceiling(playerScorecard.BasePoints * expectedMultiplier);
            Assert.That(playerScorecard.GameWeightBonusPoints, Is.EqualTo(expectedPoints));
            Assert.That(playerScorecard.GameDurationBonusPoints, Is.EqualTo(0));

            playerScorecard = actualPointsAwarded[_playerTwoId];
            Assert.That(playerScorecard.BasePoints, Is.EqualTo(_playerTwoBasePoints));
            expectedPoints = Math.Ceiling(playerScorecard.BasePoints * expectedMultiplier);
            Assert.That(playerScorecard.GameWeightBonusPoints, Is.EqualTo(expectedPoints));
            Assert.That(playerScorecard.GameDurationBonusPoints, Is.EqualTo(0));
        }
    }
}
