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
    public class HolidayGamerAchievementTests
    {
        private RhinoAutoMocker<HolidayGamerAchievement> _autoMocker;
        private readonly int _playerId = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<HolidayGamerAchievement>();
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

        [Test]
        public void ItFiltersOutInvalidResults()
        {
            //--arrange
            var playerGameResults = MakePlayerGameResults(_playerId, 3);

            //--wrong dates
            playerGameResults[0].PlayedGame.DatePlayed = new DateTime(2016, 12, 23);
            playerGameResults[1].PlayedGame.DatePlayed = new DateTime(2016, 1, 2);
            //--wrong player
            playerGameResults[2].PlayerId = -1;

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        private void SetupGamesForPlayer(int playerId, int numberOfGamesToSetUp)
        {
            var playerGameResults = MakePlayerGameResults(playerId, numberOfGamesToSetUp);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());
        }

        private static List<PlayerGameResult> MakePlayerGameResults(int playerId, int numberOfGamesToSetUp)
        {
            var playerGameResults = new List<PlayerGameResult>();

            for (int i = 0; i < numberOfGamesToSetUp; i++)
            {
                DateTime datePlayed;
                if (i == 0)
                {
                    datePlayed = new DateTime(2016, 1, 1);
                }
                else
                {
                    datePlayed = new DateTime(2016, 12, 24 + (i%7));
                }
                playerGameResults.Add(
                    new PlayerGameResult
                    {
                        PlayerId = playerId,
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = datePlayed
                        }
                    });
            }
            return playerGameResults;
        }
    }
}
