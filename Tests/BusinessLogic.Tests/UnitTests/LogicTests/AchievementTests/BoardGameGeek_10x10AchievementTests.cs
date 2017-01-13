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
    public class BoardGameGeek_10x10AchievementTests
    {
        private RhinoAutoMocker<BoardGameGeek2016_10x10Achievement> _autoMocker;
        private readonly int _playerId = 1;
        private int _targetNumberOfDistinctGames;
        private int _numberOfTimesEachGameMustBePlayed;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<BoardGameGeek2016_10x10Achievement>();
            _targetNumberOfDistinctGames = _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold];
            _numberOfTimesEachGameMustBePlayed = _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold];
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerHasntPlayedEnoughDistinctGames()
        {
            //--arrange
            SetupValidGames(_playerId, _targetNumberOfDistinctGames - 1, _numberOfTimesEachGameMustBePlayed);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerHasntPlayedEnoughOfEachDistinctGame()
        {
            //--arrange
            SetupValidGames(_playerId, _targetNumberOfDistinctGames, _numberOfTimesEachGameMustBePlayed - 1);
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
            var playerGameResults = MakePlayerGameResults(_playerId, _targetNumberOfDistinctGames -1, _numberOfTimesEachGameMustBePlayed);
            //setup some games that would have put the player over the threshold except for the fact that it's the wrong player id
            for (int i = 0; i < _numberOfTimesEachGameMustBePlayed; i++)
            {
                AddPlayerGameResult(_playerId - 1, playerGameResults, i + 100);
            }
            //setup some games that would have put the player over the threshold except for the fact that it's prior to 2016
            for (int i = 0; i < _numberOfTimesEachGameMustBePlayed; i++)
            {
                AddPlayerGameResult(_playerId - 1, playerGameResults, i + 100, new DateTime(2015, 12, 31));
            }
            //setup some games that would have put the player over the threshold except for the fact that it's after 2016
            for (int i = 0; i < _numberOfTimesEachGameMustBePlayed; i++)
            {
                AddPlayerGameResult(_playerId - 1, playerGameResults, i + 100, new DateTime(2017, 1, 1));
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
            SetupValidGames(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold], 10);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId);

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        private void SetupValidGames(int playerId, int numberOfGameDefinitionsToSetUp, int numberOfPlayedGamesToSetupForEachGameDefinition)
        {
            var playerGameResults = MakePlayerGameResults(playerId, numberOfGameDefinitionsToSetUp, numberOfPlayedGamesToSetupForEachGameDefinition);
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());
        }

        private List<PlayerGameResult> MakePlayerGameResults(int playerId, int numberOfGameDefinitionsToSetUp, int numberOfPlayedGamesToSetupForEachGameDefinition)
        {
            var playerGameResults = new List<PlayerGameResult>();

            for (int i = 0; i < numberOfGameDefinitionsToSetUp; i++)
            {
                for (int j = 0; j < numberOfPlayedGamesToSetupForEachGameDefinition; j++)
                {
                    AddPlayerGameResult(playerId, playerGameResults, i);
                }
            }

            return playerGameResults;
        }

        private static void AddPlayerGameResult(
            int playerId, 
            List<PlayerGameResult> playerGameResults, 
            int gameDefinitionId,
            DateTime? datePlayed = null)
        {
            playerGameResults.Add(
                                  new PlayerGameResult
                                  {
                                      PlayerId = playerId,
                                      PlayedGame = new PlayedGame
                                      {
                                          GameDefinitionId = gameDefinitionId,
                                          DatePlayed = datePlayed ?? new DateTime(2016, 1, 1)
                                      }
                                  });
        }
    }
}
