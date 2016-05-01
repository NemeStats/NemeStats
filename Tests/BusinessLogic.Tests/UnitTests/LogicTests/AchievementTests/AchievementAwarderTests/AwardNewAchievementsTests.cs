using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.AchievementAwarderTests
{
    [TestFixture]
    public class AwardNewAchievementsTests
    {
        private RhinoAutoMocker<AchievementAwarder> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AchievementAwarder>();
            _autoMocker.PartialMockTheClassUnderTest();
        }

        [Test]
        public void ItAssignsAchievementsForEachPlayer()
        {
            //--arrange
            var allAchievements = new List<Achievement>();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Achievement>())
                       .Return(allAchievements.AsQueryable());
            int player1Id = 10;
            int player2Id = 20;
            var playerIds = new List<int>
            {
                player1Id,
                player2Id
            };
            _autoMocker.ClassUnderTest.Expect(mock => mock.AwardAchievementsForPlayer(Arg<int>.Is.Anything, Arg<List<Achievement>>.Is.Anything));

            //--act
            _autoMocker.ClassUnderTest.AwardNewAchievements(playerIds);

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.AwardAchievementsForPlayer(Arg<int>.Is.Equal(player1Id), Arg<List<Achievement>>.Is.Equal(allAchievements)));
        }

        public class WhenCallingAwardAchievementsForPlayer : AwardNewAchievementsTests
        {
            [Test]
            public void ItChecksAllAchievements()
            {
                //--arrange
                var achievement1 = new Achievement
                {
                    Id = 1,
                    PlayerCalculationSql = "some sql 1"
                };
                var achievement2 = new Achievement
                {
                    Id = 2,
                    PlayerCalculationSql = "some sql 2"
                };
                var allAchievements = new List<Achievement>
                {
                    achievement1,
                    achievement2
                };
                int expectedPlayerId = 1;
                _autoMocker.ClassUnderTest.Expect(mock => mock.UpdateAchievement(Arg<int>.Is.Anything, Arg<Achievement>.Is.Anything)).Repeat.Any();

                //--act
                _autoMocker.ClassUnderTest.AwardAchievementsForPlayer(expectedPlayerId, allAchievements);

                //--assert
                _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.UpdateAchievement(Arg<int>.Is.Equal(expectedPlayerId), Arg<Achievement>.Is.Equal(achievement1)));
                _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.UpdateAchievement(Arg<int>.Is.Equal(expectedPlayerId), Arg<Achievement>.Is.Equal(achievement2)));

            }
        }

        public class WhenCallingAwardNewAchievements : AwardNewAchievementsTests
        {
            [Test]
            public void ItDoesNotAwardTheAchievementIfThePlayersAchievementScoreIsUnderTheLevel1Threshold()
            {
                //--arrange
                int playerId = 1;
                var achievement = new Achievement
                {
                    AchievementLevel1Threshold = 1
                };
                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>())
                    .Return(new List<PlayerAchievement>().AsQueryable());
                _autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerAchievementScore(playerId, achievement))
                           .Return(achievement.AchievementLevel1Threshold - 1);


                //--act
                _autoMocker.ClassUnderTest.UpdateAchievement(playerId, achievement);

                //--assert
                _autoMocker.Get<IDataContext>().AssertWasNotCalled(
                    mock => mock.Save(Arg<PlayerAchievement>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            }

            [Test]
            public void ItRemovesTheAchievementIfOneExistsAndThePlayersScoreIsUnderTheLevel1Threshold()
            {
                //--arrange
                int playerId = 1;
                var achievement = new Achievement
                {
                    AchievementLevel1Threshold = 1,
                    Id = 50
                };
                int existingPlayerAchievementId = 35;
                var playerAchievementQueryable = new List<PlayerAchievement>
                {
                    new PlayerAchievement
                    {
                        PlayerId = playerId,
                        AchievementId = achievement.Id,
                        Id = existingPlayerAchievementId
                    }
                }.AsQueryable();
                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>())
                           .Return(playerAchievementQueryable);

                _autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerAchievementScore(playerId, achievement))
                           .Return(achievement.AchievementLevel1Threshold - 1);
                
                //--act
                _autoMocker.ClassUnderTest.UpdateAchievement(playerId, achievement);

                //--assert
                _autoMocker.Get<IDataContext>().AssertWasCalled(
                    mock => mock.DeleteById<PlayerAchievement>(Arg<int>.Is.Equal(existingPlayerAchievementId), Arg<ApplicationUser>.Is.Anything));
            }

            [Test]
            public void ItDoesNothingIfThePlayerAlreadyHasThisLevelOfTheAchievement()
            {
                //--arrange
                int playerId = 1;
                var achievement = new Achievement
                {
                    AchievementLevel1Threshold = 1,
                    AchievementLevel2Threshold = 2,
                    AchievementLevel3Threshold = 3,
                    Id = 50
                };
                int existingPlayerAchievementId = 35;
                var playerAchievementQueryable = new List<PlayerAchievement>
                {
                    new PlayerAchievement
                    {
                        PlayerId = playerId,
                        AchievementId = achievement.Id,
                        Id = existingPlayerAchievementId,
                        AchievementLevel = AchievementLevelEnum.Level1
                    }
                }.AsQueryable();
                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>())
                           .Return(playerAchievementQueryable);

                _autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerAchievementScore(playerId, achievement))
                           .Return(achievement.AchievementLevel1Threshold);

                //--act
                _autoMocker.ClassUnderTest.UpdateAchievement(playerId, achievement);

                //--assert
                _autoMocker.Get<IDataContext>().AssertWasNotCalled(
                    mock => mock.Save(Arg<PlayerAchievement>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            }

            [Test]
            public void ItCreatesANewPlayerAchievementIfTheAchievementScoreIsEqualToTheLevel1ThresholdAndOneDoesNotYetExist()
            {
                //--arrange
                int playerId = 1;
                var achievement = new Achievement
                {
                    AchievementLevel1Threshold = 1,
                    AchievementLevel2Threshold = 2,
                    AchievementLevel3Threshold = 3,
                    Id = 50
                };

                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>())
                    .Return(new List<PlayerAchievement>().AsQueryable());

                _autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerAchievementScore(playerId, achievement))
                           .Return(achievement.AchievementLevel1Threshold);

                //--act
                _autoMocker.ClassUnderTest.UpdateAchievement(playerId, achievement);

                //--assert
                var arguments = _autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(
                    Arg<PlayerAchievement>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything));
                Assert.That(arguments, Is.Not.Null);
                Assert.That(arguments.Count, Is.EqualTo(1));
                var actualPlayerAchievement = arguments[0][0] as PlayerAchievement;
                Assert.That(actualPlayerAchievement, Is.Not.Null);
                Assert.That(actualPlayerAchievement.Id, Is.EqualTo(default(int)));
                Assert.That(actualPlayerAchievement.AchievementId, Is.EqualTo(achievement.Id));
                Assert.That(actualPlayerAchievement.AchievementLevel, Is.EqualTo(AchievementLevelEnum.Level1));
                Assert.That(actualPlayerAchievement.DateCreated.Date, Is.EqualTo(DateTime.UtcNow.Date));
                Assert.That(actualPlayerAchievement.PlayerId, Is.EqualTo(playerId));
            }

            [Test]
            public void ItAssignsALevelOneNewAchievementIfThePlayerPassesThreshold1ButIsUnderThreshold2()
            {
                //--arrange
                int playerId = 1;
                var achievement = new Achievement
                {
                    AchievementLevel1Threshold = 1,
                    AchievementLevel2Threshold = 2,
                    AchievementLevel3Threshold = 3,
                    Id = 50
                };
                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>())
                     .Return(new List<PlayerAchievement>().AsQueryable());
                _autoMocker.ClassUnderTest.Expect(mock => mock.GetPlayerAchievementScore(playerId, achievement))
                           .Return(achievement.AchievementLevel1Threshold);

                //--act
                _autoMocker.ClassUnderTest.UpdateAchievement(playerId, achievement);

                //--assert
                var arguments = _autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(
                    Arg<PlayerAchievement>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything));
                Assert.That(arguments, Is.Not.Null);
                Assert.That(arguments.Count, Is.EqualTo(1));
                var actualPlayerAchievement = arguments[0][0] as PlayerAchievement;
                Assert.That(actualPlayerAchievement, Is.Not.Null);
                Assert.That(actualPlayerAchievement.AchievementId, Is.EqualTo(achievement.Id));
                Assert.That(actualPlayerAchievement.AchievementLevel, Is.EqualTo(AchievementLevelEnum.Level1));
                Assert.That(actualPlayerAchievement.DateCreated.Date, Is.EqualTo(DateTime.UtcNow.Date));
                Assert.That(actualPlayerAchievement.PlayerId, Is.EqualTo(playerId));
            }

        }
    }
}
