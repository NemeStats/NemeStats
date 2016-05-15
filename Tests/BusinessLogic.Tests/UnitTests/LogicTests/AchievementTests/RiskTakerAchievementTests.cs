using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests
{
    [TestFixture]
    public class RiskTakerAchievementTests
    {
        private RhinoAutoMocker<RiskTakerAchievement> _autoMocker;
        private readonly int _playerId = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<RiskTakerAchievement>();
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerDoesNotReachTheBronzeThreshold()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze] - 1, 1);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItAwardsBronzeWhenPlayerHasExactlyBronzeNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze], 10);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Bronze));
        }

        [Test]
        public void ItAwardsSilverWhenPlayerHasExactlySilverNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Silver], 10);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Silver));
        }

        [Test]
        public void ItAwardsGoldWhenPlayerHasExactlyGoldNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold], 10);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        private void SetupGamesForPlayer(int playerId, int numberOfGamesThatMatch, int numberOfGamesThatDontMatch)
        {
            var playedGames = new List<PlayedGame>();
            int otherPlayerId = 100;

            for (int i = 0; i < numberOfGamesThatMatch; i++)
            {
                playedGames.Add(
                                new PlayedGame
                                {
                                    NumberOfPlayers = 10,
                                    PlayerGameResults = new List<PlayerGameResult>
                                    {
                                        new PlayerGameResult
                                        {
                                            PlayerId = playerId,
                                            GameRank = 1
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        }
                                    }
                                });
            }

            for (int i = 0; i < numberOfGamesThatDontMatch; i++)
            {
                playedGames.Add(
                                new PlayedGame
                                {
                                    NumberOfPlayers = 10,
                                    PlayerGameResults = new List<PlayerGameResult>
                                    {
                                        new PlayerGameResult
                                        {
                                            PlayerId = playerId,
                                            GameRank = 1
                                        },
                                        //--add a result with another player that also won
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++,
                                            GameRank = 1
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        },
                                        new PlayerGameResult
                                        {
                                            PlayerId = otherPlayerId++
                                        }
                                    }
                                });
            }

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGames.AsQueryable());
        }
    }
}
