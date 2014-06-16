using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests
{
    public class IntegrationTestBase
    {
        protected NemeStatsDbContext dbContext;
        protected PlayedGameRepository playedGameLogic;
        protected List<PlayedGame> testPlayedGames = new List<PlayedGame>();
        protected GameDefinition testGameDefinition;
        protected Player testPlayer1;
        protected string testPlayer1Name = "this is player 1 test 123abc";
        protected Player testPlayer2;
        protected string testPlayer2Name = "this is player 2 test 123abc";
        protected Player testPlayer3;
        protected string testPlayer3Name = "this is player 3 test 123abc";
        protected string testGameName = "this is a test game name 123abc";
        protected string testGameDescription = "this is a test game description 123abc";
        
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            dbContext = new NemeStatsDbContext();

            CleanUpTestData();

            testGameDefinition = new GameDefinition() { Name = testGameName, Description = testGameDescription };
            dbContext.GameDefinitions.Add(testGameDefinition);

            testPlayer1 = new Player() { Name = testPlayer1Name };
            dbContext.Players.Add(testPlayer1);
            testPlayer2 = new Player() { Name = testPlayer2Name };
            dbContext.Players.Add(testPlayer2);
            testPlayer3 = new Player() { Name = testPlayer3Name };
            dbContext.Players.Add(testPlayer3);
            dbContext.SaveChanges();

            playedGameLogic = new PlayedGameRepository(dbContext);

            List<Player> players = new List<Player>() { testPlayer1, testPlayer2 };
            List<int> playerRanks = new List<int>() { 1, 1 };
            PlayedGame playedGame = CreateTestPlayedGame(players, playerRanks);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer2, testPlayer3 };
            playerRanks = new List<int>() { 1, 2, 3 };
            playedGame = CreateTestPlayedGame(players, playerRanks);
            testPlayedGames.Add(playedGame);

            players = new List<Player>() { testPlayer1, testPlayer3 };
            playerRanks = new List<int>() { 2, 1 };
            playedGame = CreateTestPlayedGame(players, playerRanks);
            testPlayedGames.Add(playedGame);
        }

        private PlayedGame CreateTestPlayedGame(List<Player> players, List<int> correspondingPlayerRanks)
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>();

            for(int i = 0; i < players.Count(); i++)
            {
                playerRanks.Add(new PlayerRank()
                    {
                        PlayerId = players[i].Id,
                        GameRank = correspondingPlayerRanks[i]
                    });
            }

            NewlyCompletedGame newlyCompletedGame = new NewlyCompletedGame() 
                { 
                    GameDefinitionId = testGameDefinition.Id, 
                    PlayerRanks = playerRanks 
                };

            return playedGameLogic.CreatePlayedGame(newlyCompletedGame);
        }

        private void CleanUpTestData()
        {
            CleanUpPlayerGameResults();
            CleanUpPlayedGames();
            CleanUpGameDefinitions();
            CleanUpPlayerByPlayerName(testPlayer1Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer2Name, dbContext);
            CleanUpPlayerByPlayerName(testPlayer3Name, dbContext);


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

        private void CleanUpGameDefinitions()
        {
            List<GameDefinition> gameDefinitionsToDelete = (from game in dbContext.GameDefinitions
                                                            where game.Name == testGameName
                                                            select game).ToList();

            foreach (GameDefinition game in gameDefinitionsToDelete)
            {
                try
                {
                    dbContext.GameDefinitions.Remove(game);
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

        [TestFixtureTearDown]
        public void TearDown()
        {
            CleanUpTestData();
            dbContext.Dispose();
        }
    }
}
