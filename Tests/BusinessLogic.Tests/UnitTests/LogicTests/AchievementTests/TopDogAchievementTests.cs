using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests
{
    [TestFixture]
    public class TopDogAchievementTests
    {
        private RhinoAutoMocker<TopDogAchievement> _autoMocker;
        private readonly int _playerId = 1;
        private readonly int _otherId = 2;
        private int _gamingGroupId = 11;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<TopDogAchievement>();
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerIsNotThePlayerWithMostNemepoints()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId, winnerPoints: TopDogAchievement.MinNemePointsToUnlock);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            results.LevelAwarded.ShouldBeNull();
        }


        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerIsThePlayerWithMostNemepointsButHasLessThanMinNemePointsToUnlock()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId, winnerPoints: TopDogAchievement.MinNemePointsToUnlock-1);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            results.LevelAwarded.ShouldBeNull();
        }

        [Test]
        public void ItAwardsTheAchievementAndSetsTheGamingGroupGladiatorWhenThePlayerIsThePlayerWithMostNemepoints()
        {
            //--arrange
            SetupPlayedGames(winnerId: _playerId, loserId: _otherId, winnerPoints: TopDogAchievement.MinNemePointsToUnlock);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            results.LevelAwarded.ShouldBe(AchievementLevel.Silver);
            var args = _autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.AdminSave(Arg<GamingGroup>.Is.Anything));
            var firstCall = args.AssertFirstCallIsType<GamingGroup>();
            firstCall.Id.ShouldBe(_gamingGroupId);
            firstCall.GamingGroupChampionPlayerId.ShouldBe(_playerId);
        }

        private void SetupPlayedGames(int winnerId, int loserId, int winnerPoints)
        {
            var results = new List<PlayerGameResult>()
            {
                new PlayerGameResult
                {
                    PlayedGame = new PlayedGame()
                    {
                        GamingGroupId = _gamingGroupId
                    },
                    PlayerId = winnerId,
                    NemeStatsPointsAwarded = winnerPoints
                },
                new PlayerGameResult
                {
                    PlayedGame = new PlayedGame()
                    {
                        GamingGroupId = _gamingGroupId
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
                    GamingGroupId = _gamingGroupId
                }

            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(players.AsQueryable());

            var testingGamingGroup = new GamingGroup
            {
                Id = _gamingGroupId
            };
            var gamingGroups = new List<GamingGroup>
            {
                testingGamingGroup
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GamingGroup>()).Return(gamingGroups.AsQueryable());
        }

    }
}