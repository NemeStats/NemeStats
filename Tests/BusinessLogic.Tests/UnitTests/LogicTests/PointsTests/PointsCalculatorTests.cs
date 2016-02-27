using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Games.Validation;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PointsTests
{
    [TestFixture]
    public class PointsCalculatorTests
    {
        protected RhinoAutoMocker<PointsCalculator> _autoMocker;

        private const int FIRST_PLACE = 1;
        private const int SECOND_PLACE = 2;
        private const int THIRD_PLACE = 3;

        [SetUp]
        public virtual void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PointsCalculator>();
            _autoMocker.PartialMockTheClassUnderTest();
            _autoMocker.ClassUnderTest.Expect(mock => mock.AwardWeightBonusPoints(null, WeightTierEnum.Unknown)).IgnoreArguments();
            _autoMocker.ClassUnderTest.Expect(mock => mock.AwardPlayingTimeBonusPoints(null, AverageGameDurationTierEnum.Unknown)).IgnoreArguments();
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfAGivenPlayerHasMultipleRanks()
        {
            var playerRanks = new List<PlayerRank>
                {
                    new PlayerRank
                    {
                        PlayerId = 1,
                        GameRank = 1
                    },
                    new PlayerRank
                    {
                        PlayerId = 1,
                        GameRank = 2
                    }
                };

            Exception actualException = Assert.Throws<ArgumentException>(() => _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null));

            Assert.That(actualException.Message, Is.EqualTo("Each player can only have one PlayerRank record but one or more players have duplicate PlayerRank records."));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfThereAreMoreThan25Players()
        {
            var playerRanks = new List<PlayerRank>();
            for (var i = 0; i < PlayerRankValidator.MAXIMUM_NUMBER_OF_PLAYERS + 1; i++)
            {
                playerRanks.Add(new PlayerRank
                {
                    PlayerId = i,
                    GameRank = i + 1
                });
            }

            Exception actualException = Assert.Throws<ArgumentException>(() => _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null));

            Assert.That(actualException.Message, Is.EqualTo("There can be no more than 25 players."));
        }

        [Test]
        public void ItAwardsBasePoints()
        {
            //--arrange
            var expectedPlayerRanks = new List<PlayerRank>();
            _autoMocker.ClassUnderTest.Expect(mock => mock.AwardBasePoints(expectedPlayerRanks));

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(expectedPlayerRanks, null);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.AwardBasePoints(expectedPlayerRanks));
        }

        [Test]
        public void ItAwardsBonusPointsForGameWeight()
        {
            //--arrange
            decimal? bggAverageWeight = (decimal?)3.5;
            var bggGameDefinition = new BoardGameGeekGameDefinition
            {
                AverageWeight = bggAverageWeight
            };
            var expectedWeight = WeightTierEnum.Advanced;
            _autoMocker.Get<IWeightTierCalculator>().Expect(mock => mock.GetWeightTier(bggAverageWeight)).Return(expectedWeight);
            var expectedPlayerRanks = new List<PlayerRank>();
            _autoMocker.ClassUnderTest.Expect(mock => mock.AwardWeightBonusPoints(null, expectedWeight)).IgnoreArguments();

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(expectedPlayerRanks), bggGameDefinition);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.AwardWeightBonusPoints(
                Arg<Dictionary<int, PointsScorecard>>.Is.Anything, 
                Arg<WeightTierEnum>.Is.Equal(expectedWeight)));
        }

        [Test]
        public void ItDoesNotAwardsBonusPointsForGameWeightIfThereIsNoBoardGameGeekGameDefinition()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), null);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasNotCalled(mock => mock.AwardWeightBonusPoints(
                Arg<Dictionary<int, PointsScorecard>>.Is.Anything, 
                Arg<WeightTierEnum>.Is.Anything));
        }

        [Test]
        public void ItItDoesNotAwardsBonusPointsForGameWeightIfThereIsBoardGameGeekGameDefinitionAverageWeight()
        {
            //--arrange
            var bggGameDefinitionWithNoWeight = new BoardGameGeekGameDefinition
            {
                AverageWeight = null
            };

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), bggGameDefinitionWithNoWeight);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasNotCalled(mock => mock.AwardWeightBonusPoints(
                Arg<Dictionary<int, PointsScorecard>>.Is.Anything,
                Arg<WeightTierEnum>.Is.Anything));
        }

        [Test]
        public void ItAwardsBonusPointsForBoardGameGeekPlayingTime()
        {
            //--arrange
            var bggGameDefinition = new BoardGameGeekGameDefinition
            {
                PlayingTime = 1
            };
            var expectedTier = AverageGameDurationTierEnum.VeryShort;
            _autoMocker.Get<IAverageGameDurationTierCalculator>().Expect(mock => mock.GetAverageGameDurationTier(bggGameDefinition.PlayingTime)).Return(expectedTier);

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), bggGameDefinition);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.AwardPlayingTimeBonusPoints(
                   Arg<Dictionary<int, PointsScorecard>>.Is.Anything,
                   Arg<AverageGameDurationTierEnum>.Is.Equal(expectedTier)));
        }

        [Test]
        public void ItDoesNotAwardBonusPointsForBoardGameGeekPlayingTimeIfTheBoardGameGeekGameDefinitionIsNull()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), null);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasNotCalled(mock => mock.AwardPlayingTimeBonusPoints(
                   Arg<Dictionary<int, PointsScorecard>>.Is.Anything,
                   Arg<AverageGameDurationTierEnum>.Is.Anything));
        }

        [Test]
        public void ItDoesNotAwardBonusPointsForBoardGameGeekPlayingTimeIfThePlayingTimeIsNull()
        {
            //--arrange
            var bggGameDefinition = new BoardGameGeekGameDefinition
            {
                PlayingTime = null
            };

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), bggGameDefinition);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasNotCalled(mock => mock.AwardPlayingTimeBonusPoints(
                   Arg<Dictionary<int, PointsScorecard>>.Is.Anything,
                   Arg<AverageGameDurationTierEnum>.Is.Anything));
        }

        [TestFixture]
        public class WhenCalculatingBasePoints : PointsCalculatorTests
        {
            [Test]
            public void ItGivesTwoPointsToEachPlayerIfEveryoneLost()
            {
                var losingRank = 2;
                var playerRanks = new List<PlayerRank>
                {
                    new PlayerRank
                    {
                        PlayerId = 11,
                        GameRank = losingRank
                    },
                    new PlayerRank
                    {
                        PlayerId = 132,
                        GameRank = losingRank
                    },
                    new PlayerRank
                    {
                        PlayerId = 13,
                        GameRank = losingRank
                    },
                    new PlayerRank
                    {
                        PlayerId = 135,
                        GameRank = losingRank
                    }
                };

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                Assert.That(actualPointsAwarded.All(x => x.Value.BasePoints == 2));
            }

            [Datapoint]
            private static readonly int[] NUMBER_OF_PLAYERS_ARRAY =
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
            [TestCaseSource(nameof(NUMBER_OF_PLAYERS_ARRAY))]
            public void ItGivesAboutTenPointsPerPlayerWhenRanksAreEvenlyDistributed(int numberOfPlayers)
            {
                var playerRanks = new List<PlayerRank>();
                for (var i = 0; i < numberOfPlayers; i++)
                {
                    playerRanks.Add(new PlayerRank
                    {
                        GameRank = i + 1,
                        PlayerId = i
                    });
                }

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                //each player could round up at most 1 integer value
                var minimumPointsAwarded = PointsCalculator.POINTS_PER_PLAYER * numberOfPlayers;
                var maxPointsAwarded = PointsCalculator.POINTS_PER_PLAYER * numberOfPlayers + numberOfPlayers;
                Assert.That(actualPointsAwarded.Sum(x => x.Value.BasePoints), Is.InRange(minimumPointsAwarded, maxPointsAwarded));
            }

            [Test]
            [TestCaseSource(nameof(NUMBER_OF_PLAYERS_ARRAY))]
            public void ItGivesAboutTenPointsPerPlayerWhenRanksClumped(int numberOfPlayers)
            {
                var playerRanks = new List<PlayerRank>();
                for (var i = 0; i < numberOfPlayers; i++)
                {
                    playerRanks.Add(new PlayerRank
                    {
                        GameRank = (i + 1) % 2,
                        PlayerId = i
                    });
                }

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                //each player could round up at most 1 integer value
                var maxPointsAwarded = PointsCalculator.POINTS_PER_PLAYER * numberOfPlayers + numberOfPlayers;
                Assert.That(actualPointsAwarded.Sum(x => x.Value.BasePoints), Is.InRange(PointsCalculator.POINTS_PER_PLAYER, maxPointsAwarded));
            }

            [Test]
            public void ItGivesOutTenPointsToEachPlayerWhenEveryoneWins()
            {
                var playerRanks = new List<PlayerRank>();

                for (var i = 0; i < 13; i++)
                {
                    playerRanks.Add(new PlayerRank
                    {
                        GameRank = FIRST_PLACE,
                        PlayerId = i
                    });
                }

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                Assert.That(actualPointsAwarded.All(x => x.Value.BasePoints == 10));
            }

            [Test]
            public void ItWorksForOneWinnerAndOneLoser()
            {
                var playerRanks = new List<PlayerRank>();
                var winningPlayerId = 123;
                var losingPlayerId = 456;

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

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                var expectedWinnerPoints = 14;
                var expectedLoserPoints = 7;
                Assert.That(actualPointsAwarded[winningPlayerId].BasePoints, Is.EqualTo(expectedWinnerPoints));
                Assert.That(actualPointsAwarded[losingPlayerId].BasePoints, Is.EqualTo(expectedLoserPoints));
            }

            [Test]
            public void ItWorksForTwoWinnersAndOneLoser()
            {
                var playerRanks = new List<PlayerRank>();
                var winningPlayerId1 = 123;
                var winningPlayerId2 = 124;
                var losingPlayerId = 456;

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

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                var expectedWinnerPoints = 13;
                var expectedLoserPoints = 5;
                Assert.That(actualPointsAwarded[winningPlayerId1].BasePoints, Is.EqualTo(expectedWinnerPoints));
                Assert.That(actualPointsAwarded[winningPlayerId2].BasePoints, Is.EqualTo(expectedWinnerPoints));
                Assert.That(actualPointsAwarded[losingPlayerId].BasePoints, Is.EqualTo(expectedLoserPoints));
            }

            [Test]
            public void ItWorksForOneWinnerAndTwoLosers()
            {
                var playerRanks = new List<PlayerRank>();
                var winningPlayerId1 = 123;
                var losingPlayerId1 = 124;
                var losingPlayerId2 = 456;

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

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                var expectedWinnerPoints = 15;
                var expectedLoserPoints = 8;
                Assert.That(actualPointsAwarded[winningPlayerId1].BasePoints, Is.EqualTo(expectedWinnerPoints));
                Assert.That(actualPointsAwarded[losingPlayerId1].BasePoints, Is.EqualTo(expectedLoserPoints));
                Assert.That(actualPointsAwarded[losingPlayerId2].BasePoints, Is.EqualTo(expectedLoserPoints));
            }

            [Test]
            public void ItWorksForTwoWinnersTwoSecondPlayerAndOneLastPlace()
            {
                var playerRanks = new List<PlayerRank>();
                var winningPlayerId1 = 100;
                var winningPlayerId2 = 101;
                var secondPlacePlayer1 = 200;
                var secondPlacePlayer2 = 201;
                var thirdPlacePlayerId = 300;

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

                var actualPointsAwarded = _autoMocker.ClassUnderTest.CalculatePoints(playerRanks, null);

                var expectedFirstPlacePoints = 18;
                var expectedSecondPlacePoints = 7;
                var expectedThirdPlacePoints = 3;
                Assert.That(actualPointsAwarded[winningPlayerId1].BasePoints, Is.EqualTo(expectedFirstPlacePoints));
                Assert.That(actualPointsAwarded[winningPlayerId2].BasePoints, Is.EqualTo(expectedFirstPlacePoints));
                Assert.That(actualPointsAwarded[secondPlacePlayer1].BasePoints, Is.EqualTo(expectedSecondPlacePoints));
                Assert.That(actualPointsAwarded[secondPlacePlayer2].BasePoints, Is.EqualTo(expectedSecondPlacePoints));
                Assert.That(actualPointsAwarded[thirdPlacePlayerId].BasePoints, Is.EqualTo(expectedThirdPlacePoints));

            }

            //10 * (numberOfPlayers) * (fibonacciOf(numberOfPlayers + 1 - r) / fibonacciSumFrom(2, numberOfPlayers))
            //still need to make sure that ranked bucket groupings work correctly.
        }
        
        [TestFixture]
        public class WhenCalculatingWeightBonusPoints : PointsCalculatorTests
        {
            [SetUp]
            public override void SetUp()
            {
                _autoMocker = new RhinoAutoMocker<PointsCalculator>();
            }

            [Datapoint]
            private static readonly WeightTierEnum[] ALL_WEIGHTS =
            {
                WeightTierEnum.Casual,
                WeightTierEnum.Easy,
                WeightTierEnum.Advanced,
                WeightTierEnum.Challenging,
                WeightTierEnum.Hardcore
            };

            [Test]
            [TestCaseSource(nameof(ALL_WEIGHTS))]
            public void ItAwardsATwentyFivePercentBonusPerEachWeightLevelBeyondTheFirstAndRoundsUp(WeightTierEnum weight)
            {
                //--arrange
                int playerOneId = 1;
                int playerOneBasePoints = 100;
                int playerTwoId = 2;
                int playerTwoBasePoints = 1;
                var pointsScorecardDictionary = new Dictionary<int, PointsScorecard>
                {
                    {
                        playerOneId,
                        new PointsScorecard
                        {
                            BasePoints = playerOneBasePoints
                        }
                    },
                    {
                        playerTwoId,
                        new PointsScorecard
                        {
                            BasePoints = playerTwoBasePoints
                        }
                    }
                };

                //--act
                _autoMocker.ClassUnderTest.AwardWeightBonusPoints(pointsScorecardDictionary, weight);

                //--assert
                var playerOneScorecard = pointsScorecardDictionary[playerOneId];
                var playerTwoScorecard = pointsScorecardDictionary[playerTwoId];

                switch (weight)
                {
                    case WeightTierEnum.Casual:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(0));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(0));
                        break;
                    case WeightTierEnum.Easy:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(25));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(0));
                        break;
                    case WeightTierEnum.Advanced:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(50));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(1));
                        break;
                    case WeightTierEnum.Challenging:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(75));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(1));
                        break;
                    case WeightTierEnum.Hardcore:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(100));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(1));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(weight), weight, null);
                }
            }
        }

        [TestFixture]
        public class WhenCalculatingGameDurationBonusPoints : PointsCalculatorTests
        {
            [SetUp]
            public override void SetUp()
            {
                _autoMocker = new RhinoAutoMocker<PointsCalculator>();
            }

            [Datapoint]
            private static readonly AverageGameDurationTierEnum[] ALL_TIERS =
            {
                AverageGameDurationTierEnum.VeryShort,
                AverageGameDurationTierEnum.Short,
                AverageGameDurationTierEnum.Medium,
                AverageGameDurationTierEnum.Long,
                AverageGameDurationTierEnum.VeryLong
            };

            [Test]
            [TestCaseSource(nameof(ALL_TIERS))]
            public void ItAwardsAFiftyPercentBonusPerEachPlayingTimeTierBeyondTheFirstAndRoundsUp(AverageGameDurationTierEnum tier)
            {
                //--arrange
                int playerOneId = 1;
                int playerOneBasePoints = 100;
                int playerTwoId = 2;
                int playerTwoBasePoints = 1;
                var pointsScorecardDictionary = new Dictionary<int, PointsScorecard>
                {
                    {
                        playerOneId,
                        new PointsScorecard
                        {
                            BasePoints = playerOneBasePoints
                        }
                    },
                    {
                        playerTwoId,
                        new PointsScorecard
                        {
                            BasePoints = playerTwoBasePoints
                        }
                    }
                };

                //--act
                _autoMocker.ClassUnderTest.AwardPlayingTimeBonusPoints(pointsScorecardDictionary, tier);

                //--assert
                var playerOneScorecard = pointsScorecardDictionary[playerOneId];
                var playerTwoScorecard = pointsScorecardDictionary[playerTwoId];

                switch (tier)
                {
                    case AverageGameDurationTierEnum.VeryShort:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(0));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(0));
                        break;
                    case AverageGameDurationTierEnum.Short:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(50));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(1));
                        break;
                    case AverageGameDurationTierEnum.Medium:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(100));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(1));
                        break;
                    case AverageGameDurationTierEnum.Long:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(150));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(2));
                        break;
                    case AverageGameDurationTierEnum.VeryLong:
                        Assert.That(playerOneScorecard.GameWeightBonusPoints, Is.EqualTo(200));
                        Assert.That(playerTwoScorecard.GameWeightBonusPoints, Is.EqualTo(2));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
                }
            }
        }
    }
}
