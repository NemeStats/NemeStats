using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests
{
    [TestFixture]
    public class GameDurationBonusCalculatorTests
    {
        private RhinoAutoMocker<GameDurationBonusCalculator> _autoMocker;
        private Dictionary<int, PointsScorecard> _startingPointsScorecard;
        private readonly int _playerOneId = 1;
        private readonly int _playerOneBasePoints = 14;
        private readonly int _playerTwoId = 2;
        private readonly int _playerTwoBasePoints = 7;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<GameDurationBonusCalculator>();
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
        
        [Datapoint]
        private static readonly List<Tuple<int, decimal>> PLAY_TIME_TO_MULTIPLIER = new List<Tuple<int, decimal>>
        {
            new Tuple<int, decimal>(0, -.5M),
            new Tuple<int, decimal>(29, -.5M),
            new Tuple<int, decimal>(30, 0M),
            new Tuple<int, decimal>(59, 0M),
            new Tuple<int, decimal>(60, 1M),
            new Tuple<int, decimal>(89, 1M),
            new Tuple<int, decimal>(90, 1.9M),
            new Tuple<int, decimal>(119, 1.9M),
            new Tuple<int, decimal>(120, 2.7M),
            new Tuple<int, decimal>(149, 2.7M),
            new Tuple<int, decimal>(150, 3.4M),
            new Tuple<int, decimal>(179, 3.4M),
            new Tuple<int, decimal>(180, 4M),
            new Tuple<int, decimal>(209, 4M),
            new Tuple<int, decimal>(210, 4.5M),
            new Tuple<int, decimal>(239, 4.5M),
            new Tuple<int, decimal>(240, 4.9M),
            new Tuple<int, decimal>(269, 4.9M),
            new Tuple<int, decimal>(270, 5.2M),
            new Tuple<int, decimal>(299, 5.2M),
            new Tuple<int, decimal>(300, 5.4M),
            new Tuple<int, decimal>(329, 5.4M),
            new Tuple<int, decimal>(330, 5.5M),
            new Tuple<int, decimal>(359, 5.5M),
            new Tuple<int, decimal>(360, 5.5M),
            new Tuple<int, decimal>(389, 5.5M),
            new Tuple<int, decimal>(390, 5.5M),
            new Tuple<int, decimal>(600, 5.5M)
        };

        [Test]
        [TestCaseSource(nameof(PLAY_TIME_TO_MULTIPLIER))]
        public void ItAwardsADiminishingMultiplierDependingOnTheNumerOfHalfHourIncrements(Tuple<int, decimal> playTimeToExpectedMultplier)
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.CalculateGameDurationBonus(_startingPointsScorecard, playTimeToExpectedMultplier.Item1);

            //--assert
            AssertPercentageBonusApplied(_startingPointsScorecard, playTimeToExpectedMultplier.Item2);
        }

        private void AssertPercentageBonusApplied(Dictionary<int, PointsScorecard> actualPointsAwarded, decimal expectedMultiplier)
        {
            var playerScorecard = actualPointsAwarded[_playerOneId];
            Assert.That(playerScorecard.BasePoints, Is.EqualTo(_playerOneBasePoints));
            var expectedPoints = Math.Ceiling(playerScorecard.BasePoints * expectedMultiplier);
            Assert.That(playerScorecard.GameWeightBonusPoints, Is.EqualTo(0));
            Assert.That(playerScorecard.GameDurationBonusPoints, Is.EqualTo(expectedPoints));

            playerScorecard = actualPointsAwarded[_playerTwoId];
            Assert.That(playerScorecard.BasePoints, Is.EqualTo(_playerTwoBasePoints));
            expectedPoints = Math.Ceiling(playerScorecard.BasePoints * expectedMultiplier);
            Assert.That(playerScorecard.GameWeightBonusPoints, Is.EqualTo(0));
            Assert.That(playerScorecard.GameDurationBonusPoints, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void ItAwardsNoBonusOrPenaltyIfThePlayTimeisUnknown()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.CalculateGameDurationBonus(_startingPointsScorecard, null);

            //--assert
            AssertPercentageBonusApplied(_startingPointsScorecard, 1);
        }
    }
}
