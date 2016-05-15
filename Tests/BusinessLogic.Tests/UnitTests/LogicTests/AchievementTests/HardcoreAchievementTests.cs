using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests
{
    [TestFixture]
    public class HardcoreAchievementTests
    {
        private RhinoAutoMocker<HardcoreAchievement> _autoMocker;
        private readonly int _playerId = 1;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<HardcoreAchievement>();
        }

        [Test]
        public void ItDoesNotAwardTheAchievementWhenThePlayerDoesNotReachTheBronzeThreshold()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze] - 1);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId, _autoMocker.Get<IDataContext>());

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }

        [Test]
        public void ItFiltersOutTheAppropriateGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, 0, true);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId, _autoMocker.Get<IDataContext>());

            //--assert
            Assert.That(results.LevelAwarded, Is.Null);
        }


        [Test]
        public void ItAwardsBronzeWhenPlayerHasExactlyBronzeNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Bronze]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId, _autoMocker.Get<IDataContext>());

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Bronze));
        }

        [Test]
        public void ItAwardsSilverWhenPlayerHasExactlySilverNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Silver]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId, _autoMocker.Get<IDataContext>());

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Silver));
        }

        [Test]
        public void ItAwardsGoldWhenPlayerHasExactlyGoldNumberOfPlayedGames()
        {
            //--arrange
            SetupGamesForPlayer(_playerId, _autoMocker.ClassUnderTest.LevelThresholds[AchievementLevel.Gold]);

            //--act
            var results = _autoMocker.ClassUnderTest.IsAwardedForThisPlayer(_playerId, _autoMocker.Get<IDataContext>());

            //--assert
            Assert.That(results.LevelAwarded, Is.EqualTo(AchievementLevel.Gold));
        }

        private void SetupGamesForPlayer(int playerId, int numberOfGamesToSetUp, bool includeInvalidGames = false)
        {
            var playerGameResults = new List<PlayerGameResult>();

            for (int i = 0; i < numberOfGamesToSetUp; i++)
            {
                AddPlayerGameResult(
                    playerId, 
                    playerGameResults, 
                    i, 
                    WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE, 
                    HardcoreAchievement.THREE_HOURS_WORTH_OF_MINUTES);
            }

            if (includeInvalidGames)
            {
                //--game that has the wrong player
                AddPlayerGameResult(
                    playerId + 999,
                    playerGameResults,
                    1,
                    WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE,
                    HardcoreAchievement.THREE_HOURS_WORTH_OF_MINUTES);

                //--game that has the wrong weight
                AddPlayerGameResult(
                    playerId,
                    playerGameResults,
                    1,
                    WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE - 1,
                    HardcoreAchievement.THREE_HOURS_WORTH_OF_MINUTES);

                //--game that has the wrong duration
                AddPlayerGameResult(
                    playerId,
                    playerGameResults,
                    1,
                    WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE,
                    HardcoreAchievement.THREE_HOURS_WORTH_OF_MINUTES - 1);
            }

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());
        }

        private static void AddPlayerGameResult(
            int playerId, 
            List<PlayerGameResult> playerGameResults, 
            int gameDefinitionId, 
            decimal averageWeight, 
            int averageDuration)
        {
            playerGameResults.Add(
                                  new PlayerGameResult
                                  {
                                      PlayerId = playerId,
                                      PlayedGame = new PlayedGame
                                      {
                                          GameDefinitionId = gameDefinitionId,
                                          GameDefinition = new GameDefinition
                                          {
                                              BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
                                              {
                                                  AverageWeight = averageWeight,
                                                  MinPlayTime = averageDuration,
                                                  MaxPlayTime = averageDuration
                                              }
                                          }
                                      }
                                  });
        }
    }
}
