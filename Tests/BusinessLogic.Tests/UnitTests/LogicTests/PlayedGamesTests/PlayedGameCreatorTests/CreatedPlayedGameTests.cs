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
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
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

            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = GAMING_GROUP_ID,
                AnonymousClientId = "anonymous client id"
            };
            gameDefinition = new GameDefinition(){ Name = "game definition name", GamingGroupId = GAMING_GROUP_ID, Id = 9598 };
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
            var playerOneId = 3515;
            var playerTwoId = 15151;
            var playerOneRank = 1;
            var playerTwoRank = 2;
            var newlyCompletedGame = new NewlyCompletedGame() { GameDefinitionId = gameDefinition.Id };
            var playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = playerOneId, GameRank = playerOneRank });
            playerRanks.Add(new PlayerRank() { PlayerId = playerTwoId, GameRank = playerTwoRank });
            newlyCompletedGame.PlayerRanks = playerRanks;

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinition.Id
                                                    && game.NumberOfPlayers == playerRanks.Count()
                                                    && game.DatePlayed.Date.Equals(newlyCompletedGame.DatePlayed.Date)),
                                                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsNemeStatsPointsForEachPlayerGameResult()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerOneGameRank = 1;
            var playerTwoGameRank = 2;
            var playerRanks = new List<PlayerRank>()
            {
                new PlayerRank()
                {
                    PlayerId = playerOneId,
                    GameRank = playerOneGameRank
                },
                new PlayerRank()
                {
                    PlayerId = playerTwoId,
                    GameRank = playerTwoGameRank
                }
            };
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            var pointsDictionary = PointsCalculator.CalculatePoints(playerRanks);

            var playerOneExpectednemeStatsPoints = pointsDictionary[playerOneId];
            var user = new ApplicationUser();

            var playedGame = autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            Assert.AreEqual(playerOneExpectednemeStatsPoints, playedGame.PlayerGameResults
                                                    .First(gameResult => gameResult.PlayerId == playerOneId)
                                                    .NemeStatsPointsAwarded);

            var playerTwoExpectednemeStatsPoints = pointsDictionary[playerTwoId];
            Assert.AreEqual(playerTwoExpectednemeStatsPoints, playedGame.PlayerGameResults
                                                    .First(gameResult => gameResult.PlayerId == playerTwoId)
                                                    .NemeStatsPointsAwarded);
        }

        [Test]
        public void ItSetsTheGamingGroupIdToTheSpecifiedOne()
        {
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>(),
                GamingGroupId = 39
            };

            autoMocker.ClassUnderTest.Expect(logic => logic.TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame))
                .Repeat.Once()
                .Return(new List<PlayerGameResult>());

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<PlayedGame>.Matches(game => game.GamingGroupId == newlyCompletedGame.GamingGroupId),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheGamingGroupIdToThatOfTheUserIfOneIsNotSpecified()
        {
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>()
            };

            autoMocker.ClassUnderTest.Expect(logic => logic.TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame))
                .Repeat.Once()
                .Return(new List<PlayerGameResult>());

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<PlayedGame>.Matches(game => game.GamingGroupId == currentUser.CurrentGamingGroupId),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItRecordsAGamePlayedEvent()
        {
            var playerRank = new PlayerRank()
            {
                GameRank = 1,
                PlayerId = 1
            };
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>() { playerRank }
            };
            var transactionSource = TransactionSource.RestApi;

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, transactionSource, this.currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayedGame(currentUser, transactionSource));
        }

        [Test]
        public void ItRecalculatesTheNemesisOfEveryPlayerInTheGame()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerThreeId = 3;
            var playerRanks = new List<PlayerRank>()
            {
                new PlayerRank()
                {
                    PlayerId = playerOneId,
                    GameRank = 1
                },
                new PlayerRank()
                {
                    PlayerId = playerTwoId,
                    GameRank = 2
                },
                new PlayerRank()
                {
                    PlayerId = playerThreeId,
                    GameRank = 3
                }
            };
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            foreach(var playerRank in playerRanks)
            {
                autoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(playerRank.PlayerId, currentUser));
            }
        }

        [Test]
        public void ItRecalculatesTheChampionForTheGame()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            autoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion((int)newlyCompletedGame.GameDefinitionId, currentUser));
        }

        [Test]
        public void ItChecksSecurityOnThePlayerId()
        {
            var playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank{ PlayerId = existingPlayerWithMatchingGamingGroup.Id });
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(existingPlayerWithMatchingGamingGroup.Id))
                .Return(existingPlayerWithMatchingGamingGroup);

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);
 
            autoMocker.Get<ISecuredEntityValidator<Player>>().AssertWasCalled(mock => mock.ValidateAccess(
                existingPlayerWithMatchingGamingGroup, 
                currentUser, typeof(Player), 
                existingPlayerWithMatchingGamingGroup.Id));
        }

        [Test]
        public void ItChecksSecurityOnTheGameDefinitionId()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, this.currentUser);

            autoMocker.Get<ISecuredEntityValidator<GameDefinition>>().AssertWasCalled(mock => mock.ValidateAccess(
                gameDefinition,
                currentUser, typeof(GameDefinition),
                gameDefinition.Id));
        }
    }
}
