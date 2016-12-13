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
    public class BoardGameGeek2016_10x10AchievementTests
    {
        private RhinoAutoMocker<BoardGameGeek2016_10x10Achievement> _autoMocker;
        private readonly int _playerId = 1;
        private readonly string _applicationUserId = "the user's id";
        private int _targetNumberOfDistinctGames;
        private int _numberOfTimesEachGameMustBePlayed;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<BoardGameGeek2016_10x10Achievement>();
            _targetNumberOfDistinctGames = _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold];
            _numberOfTimesEachGameMustBePlayed = _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold];
            _autoMocker.Get<IDataContext>()
                .Expect(mock => mock.FindById<Player>(_playerId))
                .Return(new Player {ApplicationUserId = _applicationUserId});
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerHasntPlayedEnoughDistinctGames()
        {
            //--arrange
            SetupValidGames(_targetNumberOfDistinctGames - 1, _numberOfTimesEachGameMustBePlayed);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerHasntPlayedEnoughOfEachDistinctGame()
        {
            //--arrange
            SetupValidGames(_targetNumberOfDistinctGames, _numberOfTimesEachGameMustBePlayed - 1);
            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItFiltersOutTheAppropriateGames()
        {
            //--arrange
            //setup 9 games that are played 10 times
            var playerGameResults = MakePlayerGameResults(_targetNumberOfDistinctGames -1, _numberOfTimesEachGameMustBePlayed);
            //setup some games that would have put the player over the threshold except for the fact that the applicationUserId is wrong
            for (int i = 0; i < _numberOfTimesEachGameMustBePlayed; i++)
            {
                AddPlayerGameResult(playerGameResults, 500, "invalidUserId");
            }
            //too old date played
            var tooOldDate = new DateTime(2015, 12, 31);
            for (int i = 0; i < _numberOfTimesEachGameMustBePlayed; i++)
            {
                AddPlayerGameResult(playerGameResults, 600, _applicationUserId, tooOldDate);
            }

            //too new date played
            var tooNewDate = new DateTime(2017, 1, 1);
            for (int i = 0; i < _numberOfTimesEachGameMustBePlayed; i++)
            {
                AddPlayerGameResult(playerGameResults, 700, _applicationUserId, tooNewDate);
            }

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }


        [Test]
        public void ItAwardsGoldWhenPlayerHasExactlyGoldNumberOfPlayedGames()
        {
            //--arrange
            SetupValidGames(_autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold], 10);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        [Test]
        public void ItIncludesPlayedGamesSoLongAsTheApplicationUserIdMatches()
        {
            //--arrange
            var playerGameResults = MakePlayerGameResults(_autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold], 10);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());
            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        private void SetupValidGames(int numberOfGameDefinitionsToSetUp, int numberOfPlayedGamesToSetupForEachGameDefinition)
        {
            var playerGameResults = MakePlayerGameResults(numberOfGameDefinitionsToSetUp, numberOfPlayedGamesToSetupForEachGameDefinition);
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());
        }

        private List<PlayerGameResult> MakePlayerGameResults(int numberOfGameDefinitionsToSetUp, int numberOfPlayedGamesToSetupForEachGameDefinition)
        {
            var playerGameResults = new List<PlayerGameResult>();

            for (int i = 0; i < numberOfGameDefinitionsToSetUp; i++)
            {
                for (int j = 0; j < numberOfPlayedGamesToSetupForEachGameDefinition; j++)
                {
                    AddPlayerGameResult(playerGameResults, i, _applicationUserId);
                }
            }

            return playerGameResults;
        }

        private static void AddPlayerGameResult(
            List<PlayerGameResult> playerGameResults,
            int gameDefinitionId,
            string applicationUserId,
            DateTime? datePlayed = null)
        {
            if (datePlayed == null)
            {
                datePlayed = new DateTime(2016, 1, 1);
            }
            playerGameResults.Add(
                                  new PlayerGameResult
                                  {
                                      Player = new Player
                                      {
                                          ApplicationUserId = applicationUserId
                                      },
                                      PlayedGame = new PlayedGame
                                      {
                                          GameDefinitionId = gameDefinitionId,
                                          DatePlayed = datePlayed.Value
                                      }
                                  });
        }
    }
}
