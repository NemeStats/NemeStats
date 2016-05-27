using System;
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
    public class PunisherAchievementTests
    {
        private RhinoAutoMocker<PunisherAchievement> _autoMocker;
        private readonly int _playerId = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PunisherAchievement>();
        }

        [Test]
        public void ItOnlyIncludesPlayersThatWereActuallyANemesis()
        {
            //--the player's minion was never their nemesis
            var expectedNemesisRecords = new List<Nemesis>
            {
                new Nemesis
                {
                    NemesisPlayerId = _playerId,
                    MinionPlayerId = _playerId + 1,
                    DateCreated = DateTime.UtcNow,
                    NemesisPlayer = new Player
                    {
                        CurrentAndPriorNemeses = new List<Nemesis>()
                    }
                }
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Nemesis>()).Return(expectedNemesisRecords.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItOnlyIncludesRecordsForTheGivenPlayer()
        {
            //--the player's minion was never their nemesis
            var expectedNemesisRecords = new List<Nemesis>
            {
                new Nemesis
                {
                    NemesisPlayerId = _playerId + 1,
                    MinionPlayerId = _playerId + 2,
                    DateCreated = DateTime.UtcNow,
                    NemesisPlayer = new Player
                    {
                        CurrentAndPriorNemeses = new List<Nemesis>
                        {
                            new Nemesis
                            {
                                NemesisPlayerId = _playerId + 2,
                                MinionPlayerId = _playerId + 1,
                                DateCreated = DateTime.UtcNow.AddDays(-1)
                            }
                        }
                    }
                }
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Nemesis>()).Return(expectedNemesisRecords.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItOnlyIncludesWhereTheOtherPlayerWasActuallyANemesisInThePast()
        {
            //--the player's minion was never their nemesis
            var expectedNemesisRecords = new List<Nemesis>
            {
                new Nemesis
                {
                    NemesisPlayerId = _playerId,
                    MinionPlayerId = _playerId + 1,
                    DateCreated = DateTime.UtcNow.AddDays(-100),
                    NemesisPlayer = new Player
                    {
                        CurrentAndPriorNemeses = new List<Nemesis>
                        {
                            new Nemesis
                            {
                                NemesisPlayerId = _playerId + 1,
                                MinionPlayerId = _playerId,
                                DateCreated = DateTime.UtcNow
                            }
                        }
                    }
                }
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Nemesis>()).Return(expectedNemesisRecords.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerDoesNotReachTheBronzeThreshold()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze] - 1);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItAwardsBronzeWhenPlayerHasExactlyBronzeNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Bronze));
        }

        [Test]
        public void ItAwardsSilverWhenPlayerHasExactlySilverNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Silver]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Silver));
        }

        [Test]
        public void ItAwardsGoldWhenPlayerHasExactlyGoldNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        private void SetupGamesForPlayer(int playerId, int numberOfGamesToSetUp)
        {
            var results = new List<Nemesis>();

            for (int i = 0; i < numberOfGamesToSetUp; i++)
            {
                results.Add(
                            new Nemesis
                            {
                                NemesisPlayerId = playerId,
                                MinionPlayerId = i,
                                DateCreated = DateTime.UtcNow,
                                NemesisPlayer = new Player
                                {
                                    CurrentAndPriorNemeses = new List<Nemesis>
                                    {
                                        new Nemesis
                                        {
                                            NemesisPlayerId = i,
                                            MinionPlayerId = playerId,
                                            DateCreated = DateTime.UtcNow.AddDays(-1)
                                        }
                                    }
                                }
                            });
            }

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Nemesis>()).Return(results.AsQueryable());
        }
    }
}
