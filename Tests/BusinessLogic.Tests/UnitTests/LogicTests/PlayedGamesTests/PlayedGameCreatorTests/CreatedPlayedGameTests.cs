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
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Points;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameCreatorTests
{
    [TestFixture]
    public class CreatePlayedGameTests
    {
        private NemeStatsDataContext dataContext;
        private PlayedGameCreator playedGameCreatorPartialMock;
        private IPlayerRepository playerRepositoryMock;
        private INemesisRecalculator nemesisRecalculatorMock;
        private INemeStatsEventTracker playedGameTracker;
        private IChampionRecalculator championRecalculatorMock;
        private ApplicationUser currentUser;
        private GameDefinition gameDefinition;

        [SetUp]
        public void TestSetUp()
        {
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            playedGameTracker = MockRepository.GenerateMock<INemeStatsEventTracker>();
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            nemesisRecalculatorMock = MockRepository.GenerateMock<INemesisRecalculator>();
            championRecalculatorMock = MockRepository.GenerateMock<IChampionRecalculator>();

            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 1513,
                AnonymousClientId = "anonymous client id"
            };
            gameDefinition = new GameDefinition(){ Name = "game definition name" };
            dataContext.Expect(mock => mock.FindById<GameDefinition>(Arg<int>.Is.Anything))
                .Return(gameDefinition);
            playedGameCreatorPartialMock = MockRepository.GeneratePartialMock<PlayedGameCreator>(dataContext, playedGameTracker, playerRepositoryMock, nemesisRecalculatorMock, championRecalculatorMock);
        }

        [Test]
        public void ItSavesAPlayedGameIfThereIsAGameDefinition()
        {
            int gameDefinitionId = 1354;
            int playerOneId = 3515;
            int playerTwoId = 15151;
            int playerOneRank = 1;
            int playerTwoRank = 2;
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame() { GameDefinitionId = gameDefinitionId };
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank() { PlayerId = playerOneId, GameRank = playerOneRank });
            playerRanks.Add(new PlayerRank() { PlayerId = playerTwoId, GameRank = playerTwoRank });
            newlyCompletedGame.PlayerRanks = playerRanks;

            playedGameCreatorPartialMock.CreatePlayedGame(newlyCompletedGame, currentUser);

            dataContext.AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinitionId
                                                    && game.NumberOfPlayers == playerRanks.Count()
                                                    && game.DatePlayed.Date.Equals(newlyCompletedGame.DatePlayed.Date)),
                                                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsGordonPointsForEachPlayerGameResult()
        {
            int playerOneId = 1;
            int playerTwoId = 2;
            int playerOneGameRank = 1;
            int playerTwoGameRank = 2;
            List<PlayerRank> playerRanks = new List<PlayerRank>()
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
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = playerRanks
            };

            int playerOneExpectedGordonPoints = GordonPoints.CalculateGordonPoints(playerRanks.Count, playerOneGameRank);
            ApplicationUser user = new ApplicationUser();

            PlayedGame playedGame = playedGameCreatorPartialMock.CreatePlayedGame(newlyCompletedGame, currentUser);

            Assert.AreEqual(playerOneExpectedGordonPoints, playedGame.PlayerGameResults
                                                    .First(gameResult => gameResult.PlayerId == playerOneId)
                                                    .GordonPoints);

            int playerTwoExpectedGordonPoints = GordonPoints.CalculateGordonPoints(playerRanks.Count, playerTwoGameRank);
            Assert.AreEqual(playerTwoExpectedGordonPoints, playedGame.PlayerGameResults
                                                    .First(gameResult => gameResult.PlayerId == playerTwoId)
                                                    .GordonPoints);
        }

        [Test]
        public void ItSetsTheGamingGroupIdToThatOfTheUser()
        {
            int gameDefinitionId = 1354;
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinitionId,
                PlayerRanks = new List<PlayerRank>()
            };

            playedGameCreatorPartialMock.Expect(logic => logic.TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame))
                .Repeat.Once()
                .Return(new List<PlayerGameResult>());

            playedGameCreatorPartialMock.CreatePlayedGame(newlyCompletedGame, currentUser);

            dataContext.AssertWasCalled(mock => mock.Save(
                Arg<PlayedGame>.Matches(game => game.GamingGroupId == currentUser.CurrentGamingGroupId),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItRecordsAGamePlayedEvent()
        {
            PlayerRank playerRank = new PlayerRank()
            {
                GameRank = 1,
                PlayerId = 1
            };
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = new List<PlayerRank>() { playerRank }
            };

            playedGameCreatorPartialMock.CreatePlayedGame(newlyCompletedGame, currentUser);

            playedGameTracker.AssertWasCalled(mock => mock.TrackPlayedGame(currentUser, gameDefinition.Name, newlyCompletedGame.PlayerRanks.Count));
        }

        [Test]
        public void ItRecalculatesTheNemesisOfEveryPlayerInTheGame()
        {
            int playerOneId = 1;
            int playerTwoId = 2;
            int playerThreeId = 3;
            List<PlayerRank> playerRanks = new List<PlayerRank>()
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
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = playerRanks
            };

            PlayedGame playedGame = playedGameCreatorPartialMock.CreatePlayedGame(newlyCompletedGame, currentUser);

            foreach(PlayerRank playerRank in playerRanks)
            {
                nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(playerRank.PlayerId.Value, currentUser));
            }
        }

        [Test]
        public void ItRecalculatesTheChampionForTheGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame()
            {
                GameDefinitionId = 1,
                PlayerRanks = playerRanks
            };

            playedGameCreatorPartialMock.CreatePlayedGame(newlyCompletedGame, currentUser);

            championRecalculatorMock.AssertWasCalled(mock => mock.RecalculateChampion((int)newlyCompletedGame.GameDefinitionId, currentUser));
        }
    }
}
