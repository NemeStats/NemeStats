#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameCreatorTests
{
    [TestFixture]
    public class CreatePlayedGameTests
    {
        private RhinoAutoMocker<PlayedGameCreator> autoMocker;
        private ApplicationUser currentUser;
        private GameDefinition gameDefinition;
        private Player existingPlayerWithMatchingGamingGroup;
        private const int GAMING_GROUP_ID = 9;

        [SetUp]
        public void TestSetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayedGameCreator>();
            autoMocker.PartialMockTheClassUnderTest();

            currentUser = new ApplicationUser
            {
                Id = "user id",
                CurrentGamingGroupId = GAMING_GROUP_ID,
                AnonymousClientId = "anonymous client id"
            };
            gameDefinition = new GameDefinition
            { Name = "game definition name", GamingGroupId = GAMING_GROUP_ID, Id = 9598 };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GameDefinition>(gameDefinition.Id))
                .Return(gameDefinition);

            existingPlayerWithMatchingGamingGroup = new Player
            {
                Id = 1,
                GamingGroupId = GAMING_GROUP_ID
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(Arg<int>.Is.Anything)).Return(existingPlayerWithMatchingGamingGroup);
        }

        [Test]
        public void ItSavesAPlayedGameIfThereIsAGameDefinition()
        {
            autoMocker.ClassUnderTest.CreatePlayedGame(ValidNewlyCompletedGame(), TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinition.Id
                                                    && game.NumberOfPlayers == ValidNewlyCompletedGame().PlayerRanks.Count()
                                                    && game.DatePlayed.Date.Equals(ValidNewlyCompletedGame().DatePlayed.Date)),
                                                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        private NewlyCompletedGame ValidNewlyCompletedGame()
        {
            List<PlayerRank> playerRanks;

            var playerOneId = 3515;
            var playerTwoId = 15151;
            var playerOneRank = 1;
            var playerTwoRank = 2;
            var newlyCompletedGame = new NewlyCompletedGame
            {GameDefinitionId = gameDefinition.Id};
            playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank
            {PlayerId = playerOneId, GameRank = playerOneRank});
            playerRanks.Add(new PlayerRank
            {PlayerId = playerTwoId, GameRank = playerTwoRank});
            newlyCompletedGame.PlayerRanks = playerRanks;
            autoMocker.Get<IPointsCalculator>()
                .Expect(mock => mock.CalculatePoints(null, null))
                .IgnoreArguments()
                .Return(new Dictionary<int, PointsScorecard>
                {
                    {playerOneId, new PointsScorecard()},
                    {playerTwoId, new PointsScorecard()}
                });
            return newlyCompletedGame;
        }

        [Test]
        public void ItSetsNemePointsForEachPlayerGameResult()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerOneGameRank = 1;
            var playerTwoGameRank = 2;
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = playerOneId,
                    GameRank = playerOneGameRank
                },
                new PlayerRank
                {
                    PlayerId = playerTwoId,
                    GameRank = playerTwoGameRank
                }
            };
            gameDefinition.BoardGameGeekGameDefinitionId = 11;
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };
            var expectedBoardGameGeekGameDefinition = new BoardGameGeekGameDefinition();
            autoMocker.Get<IDataContext>()
                      .Expect(mock => mock.FindById<BoardGameGeekGameDefinition>(gameDefinition.BoardGameGeekGameDefinitionId))
                      .Return(expectedBoardGameGeekGameDefinition);

            var playerOneScorecard = new PointsScorecard
            {
                BasePoints = 1,
                GameDurationBonusPoints = 2,
                GameWeightBonusPoints = 3
            };
            var playerTwoScorecard = new PointsScorecard
            {
                BasePoints = 4,
                GameDurationBonusPoints = 5,
                GameWeightBonusPoints = 6
            };
            var expectedPointsDictionary = new Dictionary<int, PointsScorecard>
            {
                {playerOneId, playerOneScorecard},
                {playerTwoId, playerTwoScorecard}
            };

            autoMocker.Get<IPointsCalculator>().Expect(mock => mock.CalculatePoints(playerRanks, expectedBoardGameGeekGameDefinition))
                .Return(expectedPointsDictionary);

            //--Act
            var playedGame = autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            var playerOne = playedGame.PlayerGameResults.First(gameResult => gameResult.PlayerId == playerOneId);
            Assert.That(playerOne.GameDurationBonusPoints, Is.EqualTo(playerOneScorecard.GameDurationBonusPoints));
            Assert.That(playerOne.GameWeightBonusPoints, Is.EqualTo(playerOneScorecard.GameWeightBonusPoints));
            Assert.That(playerOne.NemeStatsPointsAwarded, Is.EqualTo(playerOneScorecard.BasePoints));

            var playerTwo = playedGame.PlayerGameResults.First(gameResult => gameResult.PlayerId == playerTwoId);
            Assert.That(playerTwo.GameDurationBonusPoints, Is.EqualTo(playerTwoScorecard.GameDurationBonusPoints));
            Assert.That(playerTwo.GameWeightBonusPoints, Is.EqualTo(playerTwoScorecard.GameWeightBonusPoints));
            Assert.That(playerTwo.NemeStatsPointsAwarded, Is.EqualTo(playerTwoScorecard.BasePoints));
        }

        [Test]
        public void ItSetsTheGamingGroupIdToThatOfTheUser()
        {
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>()
            };

            autoMocker.ClassUnderTest.Expect(logic => logic.TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(null, null))
                .IgnoreArguments()
                .Return(new List<PlayerGameResult>());

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<PlayedGame>.Matches(game => game.GamingGroupId == currentUser.CurrentGamingGroupId),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItRecordsAGamePlayedEvent()
        {
            var playerRank = new PlayerRank
            {
                GameRank = 1,
                PlayerId = 1
            };
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>
                { playerRank }
            };
            var transactionSource = TransactionSource.RestApi;
            autoMocker.Get<IPointsCalculator>()
              .Expect(mock => mock.CalculatePoints(null, null))
              .IgnoreArguments()
              .Return(new Dictionary<int, PointsScorecard>
              {
                              {playerRank.PlayerId, new PointsScorecard()}
              });

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, transactionSource, currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayedGame(currentUser, transactionSource));
        }

        [Test]
        public void ItRecalculatesTheNemesisOfEveryPlayerInTheGame()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerThreeId = 3;
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = playerOneId,
                    GameRank = 1
                },
                new PlayerRank
                {
                    PlayerId = playerTwoId,
                    GameRank = 2
                },
                new PlayerRank
                {
                    PlayerId = playerThreeId,
                    GameRank = 3
                }
            };
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };
            autoMocker.Get<IPointsCalculator>()
                      .Expect(mock => mock.CalculatePoints(null, null))
                      .IgnoreArguments()
                      .Return(new Dictionary<int, PointsScorecard>
                      {
                          {playerOneId, new PointsScorecard()},
                          {playerTwoId, new PointsScorecard()},
                          {playerThreeId, new PointsScorecard()}
                      });

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            foreach (var playerRank in playerRanks)
            {
                autoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(playerRank.PlayerId, currentUser));
            }
        }

        [Test]
        public void ItRecalculatesTheChampionForTheGameButDoesntClearTheExistingChampion()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(newlyCompletedGame.GameDefinitionId.Value, currentUser, false));
        }

        [Test]
        public void ItChecksSecurityOnThePlayerId()
        {
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = existingPlayerWithMatchingGamingGroup.Id
                }
            };
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(existingPlayerWithMatchingGamingGroup.Id))
                .Return(existingPlayerWithMatchingGamingGroup);
            autoMocker.Get<IPointsCalculator>()
                .Expect(mock => mock.CalculatePoints(null, null))
                .IgnoreArguments()
                .Return(new Dictionary<int, PointsScorecard>
            {
                {existingPlayerWithMatchingGamingGroup.Id, new PointsScorecard()}
            });

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<ISecuredEntityValidator<Player>>().AssertWasCalled(mock => mock.ValidateAccess(
                existingPlayerWithMatchingGamingGroup,
                currentUser, typeof(Player),
                existingPlayerWithMatchingGamingGroup.Id));
        }

        [Test]
        public void ItChecksSecurityOnTheGameDefinitionId()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<ISecuredEntityValidator<GameDefinition>>().AssertWasCalled(mock => mock.ValidateAccess(
                gameDefinition,
                currentUser, typeof(GameDefinition),
                gameDefinition.Id));
        }

        [Test]
        public void It_Send_PlayedGameCreatedEvent()
        {
            autoMocker.ClassUnderTest.CreatePlayedGame(ValidNewlyCompletedGame(), TransactionSource.WebApplication, currentUser);

            //TODO: Test PlayedGameEvent was sended

        }

    }
}
