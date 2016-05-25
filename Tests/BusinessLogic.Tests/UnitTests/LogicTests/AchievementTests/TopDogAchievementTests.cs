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
    public class TopDogAchievementTests
    {
        private RhinoAutoMocker<TopDogAchievement> _autoMocker;
        private readonly int _playerId = 1;
        private readonly int _otherId = 2;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<TopDogAchievement>();
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerIsNotThePlayerWithMostNemepoints()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItAwardTheAchievementWhenThePlayerIsThePlayerWithMostNemepoints()
        {
            //--arrange
            SetupPlayedGames(winnerId: _playerId, loserId: _otherId);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Silver));
        }

        private void SetupPlayedGames(int winnerId, int loserId)
        {
            var gamingGroupId = 1;
            var results = new List<PlayerGameResult>()
            {
                new PlayerGameResult
                {
                    PlayedGame = new PlayedGame()
                    {
                        GamingGroupId = gamingGroupId
                    },
                    PlayerId = winnerId,
                    NemeStatsPointsAwarded = 10
                },
                new PlayerGameResult
                {
                    PlayedGame = new PlayedGame()
                    {
                        GamingGroupId = gamingGroupId
                    },
                    PlayerId = loserId,
                    NemeStatsPointsAwarded = 5
                },
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(results.AsQueryable());

            var players = new List<Player>
            {
                new Player
                {
                    Id = _playerId,
                    GamingGroupId = gamingGroupId
                }

            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(players.AsQueryable());
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