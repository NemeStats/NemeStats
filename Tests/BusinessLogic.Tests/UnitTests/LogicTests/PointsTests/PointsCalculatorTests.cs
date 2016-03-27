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
        public void ItAwardsBasePointsWhenThereIsNoBoardGameGeekGameDefinition()
        {
            //--arrange
            var expectedPlayerRanks = new List<PlayerRank>();
            _autoMocker.ClassUnderTest.Expect(mock => mock.AwardBasePoints(null)).IgnoreArguments();

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(expectedPlayerRanks, null);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.AwardBasePoints(expectedPlayerRanks));
        }

        [Test]
        public void ItAwardsPointsWithTheCorrectWeight()
        {
            //--arrange
            var expectedPlayerRanks = new List<PlayerRank>();
            var bggGameDefinition = new BoardGameGeekGameDefinition
            {
                AverageWeight = -1
            };

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(expectedPlayerRanks, bggGameDefinition);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.AwardBasePoints(expectedPlayerRanks));
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

        [Test]
        public void ItAwardsBonusPointsForGameWeight()
        {
            //--arrange
            var bggGameDefinition = new BoardGameGeekGameDefinition
            {
                AverageWeight = WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_EASY - (decimal)0.01
            };

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), bggGameDefinition);

            //--assert
            _autoMocker.Get<IWeightBonusCalculator>().AssertWasCalled(
                                                                      mock =>
                                                                      mock.CalculateWeightBonus(Arg<Dictionary<int, PointsScorecard>>.Is.Anything,
                                                                                                Arg<decimal?>.Is.Equal(bggGameDefinition.AverageWeight)));
        }

        [Test]
        public void ItAwardsBonusPointsForGameDuration()
        {
            //--arrange
            var bggGameDefinition = new BoardGameGeekGameDefinition
            {
                MaxPlayTime = 120,
                MinPlayTime = 100
            };

            //--act
            _autoMocker.ClassUnderTest.CalculatePoints(new List<PlayerRank>(), bggGameDefinition);

            //--assert
            _autoMocker.Get<IGameDurationBonusCalculator>().AssertWasCalled(
                                                                            mock =>
                                                                            mock.CalculateGameDurationBonus(Arg<Dictionary<int, PointsScorecard>>.Is.Anything,
                                                                                                            Arg<int?>.Is.Equal(bggGameDefinition.AveragePlayTime)));
        }
    }
}
