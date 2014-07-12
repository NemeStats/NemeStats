using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Points;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Logic;
using BusinessLogic.DataAccess.Repositories;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class CreatePlayedGameTests
    {
        private NemeStatsDbContext dbContext;
        private PlayedGameLogic playedGameLogicPartialMock;
        private DbSet<PlayedGame> playedGamesDbSet ;
        private UserContextBuilder userContextBuilder;
        private string currentUserId = "id of current user";
        private UserContext userContext;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            userContextBuilder = MockRepository.GenerateMock<UserContextBuilder>();
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 1513
            };
            userContextBuilder.Expect(builder => builder.GetUserContext(currentUserId, dbContext))
                .Repeat.Once()
                .Return(userContext);
            playedGameLogicPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkPlayedGameRepository>(dbContext);
            playedGamesDbSet = MockRepository.GenerateMock<DbSet<PlayedGame>>();
            dbContext.Expect(context => context.PlayedGames)
                .Repeat.Once()
                .Return(playedGamesDbSet);
            playedGamesDbSet.Expect(dbSet => dbSet.Add(Arg<PlayedGame>.Is.Anything));
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

            playedGameLogicPartialMock.CreatePlayedGame(newlyCompletedGame, userContext);

            dbContext.AssertWasCalled(context => context.PlayedGames);

            playedGamesDbSet.AssertWasCalled(dbSet => dbSet.Add(
                    Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinitionId
                                                && game.NumberOfPlayers == playerRanks.Count()
                                                && game.DatePlayed.Date.Equals(DateTime.UtcNow.Date))));
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
            UserContext user = new UserContext();

            PlayedGame playedGame = playedGameLogicPartialMock.CreatePlayedGame(newlyCompletedGame, userContext);

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

            playedGameLogicPartialMock.Expect(logic => logic.TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame))
                .Repeat.Once()
                .Return(new List<PlayerGameResult>());

            playedGameLogicPartialMock.CreatePlayedGame(newlyCompletedGame, userContext);

            playedGamesDbSet.AssertWasCalled(dbSet => dbSet.Add(
                    Arg<PlayedGame>.Matches(game => game.GamingGroupId == userContext.GamingGroupId)));
        }
    }
}
