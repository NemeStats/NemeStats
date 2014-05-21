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
                new Player(){Id = playerDave, Name = "Big Boss Dave"},
                new Player(){Id = playerGrant, Name = "El Granto"},
                new Player(){Id = playerGarrett, Name = "The Openshaw"},
                new Player(){Id = playerKyle, Name = "The Slink"},
                new Player(){Id = playerJoey, Name = "Gooseman"}
            };

            players.ForEach(player => dbContext.Players.Add(player));
            dbContext.SaveChanges();
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
                new PlayerRank() { GameRank = 1, PlayerId = playerDave }, 
                new PlayerRank() { GameRank = 2, PlayerId = playerGrant } 
            };

            completedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = smallWorldGameDefinitionID, PlayerRanks = playerRanks });
        }

        private void CreateRaceForTheGalaxyPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = playerDave },
                new PlayerRank() { GameRank = 2, PlayerId = playerJoey }, 
                new PlayerRank() { GameRank = 3, PlayerId = playerGarrett } 
            };

            completedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = raceForTheGalaxyGameDefinitionID, PlayerRanks = playerRanks });
        }

        private void CreateSettlersOfCatanPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = playerGarrett },
                new PlayerRank() { GameRank = 2, PlayerId = playerKyle }, 
                new PlayerRank() { GameRank = 3, PlayerId = playerJoey },
                new PlayerRank() { GameRank = 4, PlayerId = playerKyle }
            };

            completedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = settlersOfCatanGameDefinitionID, PlayerRanks = playerRanks });
        }
    }
}
