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
using BusinessLogic.Models.Games;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class CreatePlayedGameIntegrationTests
    {
        private NemeStatsDbContext dbContext;
        private PlayedGameRepository playedGameLogic;
        private PlayedGame testPlayedGame;
        private int testPlayedGameId;
        private GameDefinition testGameDefinition;
        private Player testPlayer1;
        private Player testPlayer2;
        private string testGameName = "this is a test game name 123abc";
        private string testGameDescription = "this is a test game description 123abc";
        private string testPlayer1Name = "this is player 1 test 123abc";
        private string testPlayer2Name = "this is player 2 test 123abc";

        //TODO When there are failures sometimes test data gets left in the DB. What is the best solution to this? Possibly make very unique names and descriptions and clean up
        // both during test fixture setup and teardown?

        [TestFixtureSetUp, Ignore("ignoring until I add some meaningful tests here. Until then it's a waste of time.")]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();

            CleanUpTestData();

            testGameDefinition = new GameDefinition() { Name = testGameName, Description = testGameDescription };
            dbContext.GameDefinitions.Add(testGameDefinition);

            testPlayer1 = new Player() { Name = testPlayer1Name };
            dbContext.Players.Add(testPlayer1);
            testPlayer2 = new Player() { Name = testPlayer2Name };
            dbContext.Players.Add(testPlayer2);

            dbContext.SaveChanges();
            testPlayedGameId = testGameDefinition.Id;

            playedGameLogic = new PlayedGameRepository(dbContext);

            List<Player> players = new List<Player>() { testPlayer1, testPlayer2 };
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = testPlayer1.Id }, 
                new PlayerRank() { GameRank = 1, PlayerId = testPlayer2.Id } 
            };
            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame() { GameDefinitionId = testPlayedGameId, PlayerRanks = playerRanks };

            testPlayedGame = playedGameLogic.CreatePlayedGame(newlyCompletedGame);
        }

        private void CleanUpTestData()
        {
            CleanUpPlayerGameResults();
            CleanUpPlayedGames();
            CleanUpPlayerByPlayerName(testPlayer1Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer2Name, dbContext);


            dbContext.SaveChanges();
        }

        private void CleanUpPlayedGames()
        {
            List<PlayedGame> playedGamesToDelete = (from playedGame in dbContext.PlayedGames
                                                    where playedGame.GameDefinition.Name == testGameName
                                                    select playedGame).ToList();

            foreach (PlayedGame playedGame in playedGamesToDelete)
            {
                try
                {
                    dbContext.PlayedGames.Remove(playedGame);
                }
                catch (Exception) { }
            }
        }

        private void CleanUpPlayerGameResults()
        {
            List<PlayerGameResult> playerGameResultsToDelete = (from playerGameResult in dbContext.PlayerGameResults
                                                                where playerGameResult.PlayedGame.GameDefinition.Name == testGameName
                                                                select playerGameResult).ToList();

            foreach (PlayerGameResult playerGameResult in playerGameResultsToDelete)
            {
                try
                {
                    dbContext.PlayerGameResults.Remove(playerGameResult);
                }
                catch (Exception) { }
            }
        }

        private static void CleanUpPlayerByPlayerName(string playerName, NemeStatsDbContext nemeStatsDbContext)
        {
            Player playerToDelete = nemeStatsDbContext.Players.FirstOrDefault(player => player.Name == playerName);

            if (playerToDelete != null)
            {
                try
                {
                    nemeStatsDbContext.Players.Remove(playerToDelete);
                }
                catch (Exception) { }
            }
        }

        [Test]
        public void ItCreatesATwoPlayerPlayedGameAndSetsTheNumberOfPlayers()
        {
            PlayedGame playedGameFromTheDatabase = dbContext.PlayedGames.Find(testPlayedGame.Id);

            Assert.IsTrue(playedGameFromTheDatabase.NumberOfPlayers == 2);
        }

        //TODO need more integration tests, but have been looking at the database manually and it looks OK.

        [TestFixtureTearDown]
        public void TearDown()
        {
            CleanUpTestData();
            dbContext.Dispose();
        }
    }
}
