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
    public class CompletedGameTests
    {
        private NerdScorekeeperDbContext dbContext;
        private CompletedGame playedGameLogic;
        private PlayedGame playedGame;
        private int playedGameId;
        private GameDefinition gameDefinition;
        private Player player1;
        private Player player2;

        [SetUp]
        public void SetUp()
        {
            dbContext = new NerdScorekeeperDbContext();

            gameDefinition = new GameDefinition() { Name = "TestName", Description = "TestDescription" };
            dbContext.GameDefinitions.Add(gameDefinition);

            player1 = new Player() { Name = "Player1" };
            dbContext.Players.Add(player1);
            player2 = new Player() { Name = "Player2" };
            dbContext.Players.Add(player2);

            dbContext.SaveChanges();
            playedGameId = gameDefinition.Id;

            playedGameLogic = new CompletedGame(dbContext);
        }

        [TearDown]
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
            
            dbContext.SaveChanges();
        }

        [Test]
        public void ItCreatesATwoPlayerPlayedGame()
        {
            List<Player> players = new List<Player>() { player1, player2 };
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = player1.Id }, 
                new PlayerRank() { GameRank = 1, PlayerId = player2.Id } 
            };
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame() { GameDefinitionId = playedGameId, PlayerRanks = playerRanks };

            playedGame = playedGameLogic.CreatePlayedGame(newlyCompletedGame);

            Assert.IsTrue(playedGame.NumberOfPlayers == 2);
        }
    }
}
