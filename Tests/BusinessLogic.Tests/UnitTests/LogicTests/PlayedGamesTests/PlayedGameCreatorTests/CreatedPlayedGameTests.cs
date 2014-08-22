using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
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
using System.Text;
using System.Threading.Tasks;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameCreatorTests
{
    [TestFixture]
    public class CreatePlayedGameTests
    {
        private NemeStatsDataContext dataContext;
        private PlayedGameCreatorImpl playedGameCreatorPartialMock;
        private PlayedGameTracker playedGameTracker;
        private ApplicationUser currentUser;
        private GameDefinition gameDefinition;

        [SetUp]
        public void TestSetUp()
        {
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            playedGameTracker = MockRepository.GenerateMock<PlayedGameTracker>();

            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 1513,
                AnonymousClientId = "anonymous client id"
            };
            gameDefinition = new GameDefinition(){ Name = "game definition name" };
            dataContext.Expect(mock => mock.FindById<GameDefinition>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Same(currentUser)))
                .Return(gameDefinition);
            playedGameCreatorPartialMock = MockRepository.GeneratePartialMock<PlayedGameCreatorImpl>(dataContext, playedGameTracker);
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
                                                    && game.DatePlayed.Date.Equals(DateTime.UtcNow.Date)),
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
    }
}
