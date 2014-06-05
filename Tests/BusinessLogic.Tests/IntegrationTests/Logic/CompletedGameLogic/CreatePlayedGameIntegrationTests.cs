using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Logic;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.Logic
{
    [TestFixture]
    public class CreatePlayedGameIntegrationTests
    {
        private NemeStatsDbContext dbContext;
        private CompletedGameRepository playedGameLogic;
        private PlayedGame playedGame;
        private int playedGameId;
        private GameDefinition gameDefinition;
        private Player player1;
        private Player player2;

        //TODO When there are failures sometimes test data gets left in the DB. What is the best solution to this?

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();

            gameDefinition = new GameDefinition() { Name = "TestName", Description = "TestDescription" };
            dbContext.GameDefinitions.Add(gameDefinition);

            player1 = new Player() { Name = "Player1" };
            dbContext.Players.Add(player1);
            player2 = new Player() { Name = "Player2" };
            dbContext.Players.Add(player2);

            dbContext.SaveChanges();
            playedGameId = gameDefinition.Id;

            playedGameLogic = new CompletedGameRepository(dbContext);

            List<Player> players = new List<Player>() { player1, player2 };
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = player1.Id }, 
                new PlayerRank() { GameRank = 1, PlayerId = player2.Id } 
            };
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame() { GameDefinitionId = playedGameId, PlayerRanks = playerRanks };

            playedGame = playedGameLogic.CreatePlayedGame(newlyCompletedGame);
        }

        [Test]
        public void ItCreatesATwoPlayerPlayedGame()
        {
            Assert.IsTrue(playedGame.NumberOfPlayers == 2);
        }

        //TODO need more integration tests, but have been looking at the database manually and it looks OK.

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.GameDefinitions.Remove(gameDefinition);
            try
            {
                dbContext.Players.Remove(player1);
            }
            catch (Exception) { }

            try
            {
                dbContext.Players.Remove(player2);
            }
            catch (Exception) { }

            try
            {
                dbContext.PlayedGames.Remove(playedGame);
            }
            catch (Exception) { }

            try
            {
                dbContext.SaveChanges();
            }
            finally
            {
                dbContext.Dispose();
            }

        }
    }
}
