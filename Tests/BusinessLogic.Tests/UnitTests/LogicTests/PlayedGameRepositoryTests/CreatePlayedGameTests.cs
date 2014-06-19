using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Points;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class CreatePlayedGameTests
    {
        private NemeStatsDbContext dbContext = null;
        PlayedGameLogic playedGameLogic = null;
        DbSet<PlayedGame> playedGamesDbSet = null;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            playedGameLogic = new PlayedGameRepository(dbContext);
            playedGamesDbSet = MockRepository.GenerateMock<DbSet<PlayedGame>>();
            dbContext.Expect(context => context.PlayedGames).Repeat.Once().Return(playedGamesDbSet);
            playedGamesDbSet.Expect(dbSet => dbSet.Add(Arg<PlayedGame>.Is.Anything));
        }

        [Test, Ignore("need to check this with someone who knows how to test EF stuff. "
            + "Doesn't look like I'm setting my expectations right. Also need clarification on how many separate tests there should be.")]
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

            playedGameLogic.CreatePlayedGame(newlyCompletedGame);

            dbContext.AssertWasCalled(context => context.PlayedGames);
            //TODO need grant help on this test
            playedGamesDbSet.AssertWasCalled(dbSet => dbSet.Add(
                    Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinitionId
                                                && game.NumberOfPlayers == playerRanks.Count()
                                                && game.DatePlayed.Date.Equals(DateTime.UtcNow))));
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

            PlayedGame playedGame = playedGameLogic.CreatePlayedGame(newlyCompletedGame);

            Assert.AreEqual(playerOneExpectedGordonPoints, playedGame.PlayerGameResults
                                                    .First(gameResult => gameResult.PlayerId == playerOneId)
                                                    .GordonPoints);

            int playerTwoExpectedGordonPoints = GordonPoints.CalculateGordonPoints(playerRanks.Count, playerTwoGameRank);
            Assert.AreEqual(playerTwoExpectedGordonPoints, playedGame.PlayerGameResults
                                                    .First(gameResult => gameResult.PlayerId == playerTwoId)
                                                    .GordonPoints);
        }
    }
}
