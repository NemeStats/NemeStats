using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests
{
    [TestFixture]
    public class UsurperAchievementTests
    {
        private RhinoAutoMocker<UsurperAchievement> _autoMocker;
        private readonly int _playerId = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UsurperAchievement>();
        }

        private List<int> MakeGameDefinitionIds(int numberOfElementsInList)
        {
            var list = new List<int>();
            for (int i = 0; i < numberOfElementsInList; i++)
            {
                list.Add(i);
            }
            return list;
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerDoesNotReachTheBronzeThreshold()
        {
            //--arrange
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetUsurperAchievementData(_playerId))
                .Return(MakeGameDefinitionIds(_autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze] - 1));
            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItAwardsBronzeWhenPlayerHasExactlyBronzeNumberOfPlayedGames()
        {
            //--arrange
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetUsurperAchievementData(_playerId))
                .Return(MakeGameDefinitionIds(_autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze]));

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Bronze));
        }

        [Test]
        public void ItAwardsSilverWhenPlayerHasExactlySilverNumberOfPlayedGames()
        {
            //--arrange
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetUsurperAchievementData(_playerId))
                .Return(MakeGameDefinitionIds(_autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Silver]));
            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Silver));
        }

        [Test]
        public void ItAwardsGoldWhenPlayerHasExactlyGoldNumberOfPlayedGames()
        {
            //--arrange
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetUsurperAchievementData(_playerId))
                .Return(MakeGameDefinitionIds(_autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold]));
            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }
    }
}
