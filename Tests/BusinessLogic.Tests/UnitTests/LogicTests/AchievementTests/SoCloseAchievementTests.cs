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
    public class SoCloseAchievementTests
    {
        private RhinoAutoMocker<SoCloseAchievement> _autoMocker;
        private readonly int _playerId = 1;
        private readonly int _otherId = 2;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<SoCloseAchievement>();
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerDoesNotReachTheBronzeThreshold()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId, scoredGames: _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze] - 1);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }


        [Test]
        public void ItAwardsBronzeWhenPlayerHasExactlyBronzeNumberOfPlayedGames()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId, scoredGames: _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Bronze));
        }

        [Test]
        public void ItAwardsSilverWhenPlayerHasExactlySilverNumberOfPlayedGames()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId, scoredGames: _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Silver]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Silver));
        }

        [Test]
        public void ItAwardsGoldWhenPlayerHasExactlyGoldNumberOfPlayedGames()
        {
            //--arrange
            SetupPlayedGames(winnerId: _otherId, loserId: _playerId, scoredGames: _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        private void SetupPlayedGames(int winnerId, int loserId, int scoredGames)
        {
            var gamingGroupId = 1;
            var i = 0;
            var results = new List<PlayerGameResult>();
            while (i < scoredGames)
            {
                var playedGame = i + 10;
                results.Add
                (
                    new PlayerGameResult
                    {
                        PlayedGameId = playedGame,
                        PlayedGame = new PlayedGame()
                        {
                            GamingGroupId = gamingGroupId,
                        },
                        PlayerId = winnerId,
                        GameRank = 1,
                        PointsScored = 10
                    }
                );

                results.Add
                (
                    new PlayerGameResult
                    {
                        PlayedGameId = playedGame,
                        PlayedGame = new PlayedGame()
                        {
                            GamingGroupId = gamingGroupId
                        },
                        PlayerId = loserId,
                        GameRank = 2,
                       PointsScored = 9
                    }
                );
                i++;
            }

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

    }
}