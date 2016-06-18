using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests
{
    public class MiseryLikesCompanyAchievementTests
    {
        private RhinoAutoMocker<MiseryLikesCompanyAchievement> _autoMocker;
        private readonly int _playerId = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<MiseryLikesCompanyAchievement>();
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
        public void ItDoesNotIncludeGamesWhereItWasATeamWin()
        {
            //--arrange
            var validGames = MakeValidPlayedGames(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze]);
            validGames[0].WinnerType = WinnerTypes.TeamWin;
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(validGames.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItDoesNotIncludeGamesWhereItWasAPlayerWin()
        {
            //--arrange
            var validGames = MakeValidPlayedGames(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze]);
            validGames[0].WinnerType = WinnerTypes.PlayerWin;
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(validGames.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }
        private void SetupGamesForPlayer(int playerId, int numberOfGamesToSetUp)
        {
            var results = MakeValidPlayedGames(playerId, numberOfGamesToSetUp);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(results.AsQueryable());
        }

        private static List<PlayedGame> MakeValidPlayedGames(int playerId, int numberOfGamesToSetUp)
        {
            var results = new List<PlayedGame>();

            for (int i = 0; i < numberOfGamesToSetUp; i++)
            {
                results.Add(
                    new PlayedGame
                    {
                        Id = i,
                        WinnerType = WinnerTypes.TeamLoss,
                        PlayerGameResults = new List<PlayerGameResult>
                        {
                            new PlayerGameResult
                            {
                                PlayerId = playerId
                            }
                        }
                    });
            }
            return results;
        }
    }
}
