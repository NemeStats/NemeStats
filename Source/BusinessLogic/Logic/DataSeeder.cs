using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class DataSeeder
    {
        internal static string PLAYER_NAME_DAVE = "Big Boss Dave";
        internal static string GAME_NAME_SMALL_WORLD = "Small World";

        private Player bigBossDave;
        private Player elGranto;
        private Player theOpenshaw;
        private Player theSlink;
        private Player gooseman;

        private GameDefinition smallWorld;
        private GameDefinition raceForTheGalaxy;
        private GameDefinition settlersOfCatan;

        private NemeStatsDbContext dbContext;
        private PlayedGameRepository playedGame;

        public DataSeeder(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public void SeedData()
        {
            if (dbContext.GameDefinitions.FirstOrDefault(x => x.Name == GAME_NAME_SMALL_WORLD) == null)
            {
                playedGame = new PlayedGameRepository(dbContext);

                CreateGameDefinitions();
                CreatePlayers();
                CreatePlayedGames();
            }
        }

        private void CreateGameDefinitions()
        {
            smallWorld = new GameDefinition()
            {
                Name = "Small World",
                Description = "Dominate the small world."
            };
            raceForTheGalaxy = new GameDefinition() { Name = "Race For The Galaxy", Description = "Win the race for the galaxy." };
            settlersOfCatan = new GameDefinition() { Name = "Settlers of Catan", Description = "Go settle Catan." };
            var gameDefinitions = new List<GameDefinition>
            {
                smallWorld,
                raceForTheGalaxy,
                settlersOfCatan
            };

            gameDefinitions.ForEach(game => dbContext.GameDefinitions.Add(game));
            dbContext.SaveChanges();
        }

        private void CreatePlayers()
        {
            bigBossDave = new Player() { Name = PLAYER_NAME_DAVE };
            elGranto = new Player() { Name = "El Granto" };
            theOpenshaw = new Player() { Name = "The Openshaw" };
            theSlink = new Player() { Name = "The Slink" };
            gooseman = new Player() { Name = "Gooseman" };

            var players = new List<Player>
            {
                bigBossDave,
                elGranto,
                theOpenshaw,
                theSlink,
                gooseman
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
                new PlayerRank() { GameRank = 1, PlayerId =bigBossDave.Id }, 
                new PlayerRank() { GameRank = 2, PlayerId = elGranto.Id } 
            };

            playedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = smallWorld.Id, PlayerRanks = playerRanks });
        }

        private void CreateRaceForTheGalaxyPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = theSlink.Id },
                new PlayerRank() { GameRank = 2, PlayerId = theOpenshaw.Id }, 
                new PlayerRank() { GameRank = 3, PlayerId = elGranto.Id } 
            };

            playedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = raceForTheGalaxy.Id, PlayerRanks = playerRanks });
        }

        private void CreateSettlersOfCatanPlayedGame()
        {
            List<PlayerRank> playerRanks = new List<PlayerRank>() 
            { 
                new PlayerRank() { GameRank = 1, PlayerId = gooseman.Id },
                new PlayerRank() { GameRank = 2, PlayerId = theOpenshaw.Id }, 
                new PlayerRank() { GameRank = 3, PlayerId = bigBossDave.Id },
                new PlayerRank() { GameRank = 4, PlayerId = elGranto.Id }
            };

            playedGame.CreatePlayedGame(new NewlyCompletedGame() { GameDefinitionId = settlersOfCatan.Id, PlayerRanks = playerRanks });
        }
    }
}
