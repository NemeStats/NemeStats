using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class DataSeeder
    {
        private int smallWorldGameDefinitionID = 1;
        private int raceForTheGalaxyGameDefinitionID = 2;
        private int settlersOfCatanGameDefinitionID = 3;

        private List<Player> savedPlayers;

        private int playerDave = 10;
        private int playerGrant = 11;
        private int playerGarrett = 12;
        private int playerKyle = 13;
        private int playerJoey = 14;

        private NerdScorekeeperDbContext dbContext;
        private CompletedGameLogic completedGame;

        public DataSeeder(NerdScorekeeperDbContext context)
        {
            dbContext = context;
        }

        public void SeedData()
        {
            completedGame = new CompletedGameLogic(dbContext);

            CreateGameDefinitions();
            CreatePlayers();
            CreatePlayedGames();
        }

        private void CreateGameDefinitions()
        {
            var gameDefinitions = new List<GameDefinition>
            {
                new GameDefinition(){Id = smallWorldGameDefinitionID, Name = "Small World", Description="Dominate the small world."},
                new GameDefinition(){Id = raceForTheGalaxyGameDefinitionID, Name = "Race For The Galaxy", Description="Win the race for the galaxy."},
                new GameDefinition(){Id = settlersOfCatanGameDefinitionID, Name = "Settlers of Catan", Description="Go settle Catan."}
            };

            gameDefinitions.ForEach(game => dbContext.GameDefinitions.Add(game));
            dbContext.SaveChanges();
        }

        private void CreatePlayers()
        {
            var players = new List<Player>
            {
                new Player(){ Name = "Big Boss Dave"},
                new Player(){ Name = "El Granto"},
                new Player(){ Name = "The Openshaw"},
                new Player(){ Name = "The Slink"},
                new Player(){ Name = "Gooseman"}
            };

            players.ForEach(player => dbContext.Players.Add(player));
            dbContext.SaveChanges();

            savedPlayers = players;
        }

        private void CreatePlayedGames()
        {
            CreateSmallWorldPlayedGame();
            CreateRaceForTheGalaxyPlayedGame();
            CreateSettlersOfCatanPlayedGame();
        }

        private void CreateSmallWorldPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = savedPlayers[0].Id }, 
                new PlayerRank() { GameRank = 2, PlayerId = savedPlayers[1].Id } 
            };

            completedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = smallWorldGameDefinitionID, PlayerRanks = playerRanks });
        }

        private void CreateRaceForTheGalaxyPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = savedPlayers[2].Id },
                new PlayerRank() { GameRank = 2, PlayerId = savedPlayers[3].Id }, 
                new PlayerRank() { GameRank = 3, PlayerId = savedPlayers[4].Id } 
            };

            completedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = raceForTheGalaxyGameDefinitionID, PlayerRanks = playerRanks });
        }

        private void CreateSettlersOfCatanPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = savedPlayers[4].Id },
                new PlayerRank() { GameRank = 2, PlayerId = savedPlayers[2].Id }, 
                new PlayerRank() { GameRank = 3, PlayerId = savedPlayers[1].Id },
                new PlayerRank() { GameRank = 4, PlayerId = savedPlayers[3].Id }
            };

            completedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = settlersOfCatanGameDefinitionID, PlayerRanks = playerRanks });
        }
    }
}
