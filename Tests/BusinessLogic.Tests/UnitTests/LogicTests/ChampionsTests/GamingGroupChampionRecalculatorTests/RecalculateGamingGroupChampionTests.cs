using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.ChampionsTests.GamingGroupChampionRecalculatorTests
{
    public class RecalculateGamingGroupChampionTests
    {
        private AutoMocker<GamingGroupChampionRecalculator> _autoMocker;
        private int _playedGameId = 1;
        private int _gamingGroupId = 2;
        private int _playerId = 3;
        private GamingGroup _gamingGroup;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<GamingGroupChampionRecalculator>();

            _gamingGroup = new GamingGroup
            {
                Id = _gamingGroupId
            };
            var gamingGroupsQueryable = new List<GamingGroup>
            {
                _gamingGroup

            }.AsQueryable();
            _autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GamingGroup>())
                .Return(gamingGroupsQueryable);
        }

        [Test]
        public void It_Throws_An_Argument_Exception_If_The_PlayedGameId_Is_Invalid()
        {
            //--arrange
            var playerGameResults = new List<PlayerGameResult>();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());

            var invalidPlayedGameId = -1;
            var expectedException = new ArgumentException($"PlayedGame with id '{invalidPlayedGameId}' does not exist.");

            //--act
            var actualException = Assert.Throws<ArgumentException>(() => _autoMocker.ClassUnderTest.RecalculateGamingGroupChampion(invalidPlayedGameId));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void It_Sets_The_Gaming_Group_Champion_When_A_Player_Has_The_Most_NemePoints_In_The_Gaming_Group_And_They_Have_The_Minimum_Number_Of_Points()
        {
            //--arrange
            var playerGameResults = new List<PlayerGameResult>
            {
                //--making two games to get to the total to make sure it sums correctly
                MakePlayerGameResult(_playerId, GamingGroupChampionRecalculator.MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION / 2),
                MakePlayerGameResult(_playerId, GamingGroupChampionRecalculator.MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION / 2)
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());

            //--act
            _autoMocker.ClassUnderTest.RecalculateGamingGroupChampion(_playedGameId);

            //--assert
            var args = _autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.AdminSave(Arg<GamingGroup>.Is.Anything));

            var savedGamingGroup = args.AssertFirstCallIsType<GamingGroup>();
            savedGamingGroup.Id.ShouldBe(_gamingGroupId);
            savedGamingGroup.GamingGroupChampionPlayerId.ShouldBe(_playerId);
        }

        [Test]
        public void It_Sets_The_Champion_To_The_Player_With_The_Lower_PlayerId_When_There_Is_A_Tie()
        {
            //--arrange
            var playerGameResults = new List<PlayerGameResult>
            {
                MakePlayerGameResult(_playerId + 9999, GamingGroupChampionRecalculator.MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION),
                //--this one has the lower player id and should get precedence
                MakePlayerGameResult(_playerId, GamingGroupChampionRecalculator.MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION)
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());

            //--act
            _autoMocker.ClassUnderTest.RecalculateGamingGroupChampion(_playedGameId);

            //--assert
            var args = _autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.AdminSave(Arg<GamingGroup>.Is.Anything));

            var savedGamingGroup = args.AssertFirstCallIsType<GamingGroup>();
            savedGamingGroup.Id.ShouldBe(_gamingGroupId);
            savedGamingGroup.GamingGroupChampionPlayerId.ShouldBe(_playerId);
        }

        [Test]
        public void It_Doesnt_Award_When_The_Player_Has_Less_Than_The_Minimum_NemePoints()
        {
            //--arrange
            var playerGameResults = new List<PlayerGameResult>
            {
                MakePlayerGameResult(_playerId, GamingGroupChampionRecalculator.MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION - 1)
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());

            //--act
            _autoMocker.ClassUnderTest.RecalculateGamingGroupChampion(_playedGameId);

            //--assert
           _autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.AdminSave(Arg<GamingGroup>.Is.Anything));
        }

        private PlayerGameResult MakePlayerGameResult(int playerId, int totalPoints)
        {
            return new PlayerGameResult
            {
                PlayedGameId = _playedGameId,
                PlayerId = playerId,
                GameWeightBonusPoints = 0,
                GameDurationBonusPoints = 0,
                NemeStatsPointsAwarded = totalPoints,
                PlayedGame = new PlayedGame
                {
                    GamingGroupId = _gamingGroupId
                }
            };
        }
    }
}
