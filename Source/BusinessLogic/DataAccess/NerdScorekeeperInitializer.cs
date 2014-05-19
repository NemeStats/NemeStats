using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public class NerdScorekeeperInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<NerdScorekeeperDbContext>
    {
        private int smallWorldGameDefinitionID = 1;
        private int raceForTheGalaxyGameDefinitionID = 2;
        private int settlersOfCatanGameDefinitionID = 3;

        private int playerDave = 10;
        private int playerGrant = 11;
        private int playerGarrett = 12;
        private int playerKyle = 13;
        private int playerJoey = 14;

        protected override void Seed(NerdScorekeeperDbContext context)
        {
            var gameDefinitions = new List<GameDefinition>
            {
                new GameDefinition(){Id = smallWorldGameDefinitionID, Name = "Small World", Description="Dominate the small world."},
                new GameDefinition(){Id = raceForTheGalaxyGameDefinitionID, Name = "Race For The Galaxy", Description="Win the race for the galaxy."},
                new GameDefinition(){Id = settlersOfCatanGameDefinitionID, Name = "Settlers of Catan", Description="Go settle Catan."}
            };

            gameDefinitions.ForEach(game => context.GameDefinitions.Add(game));
            context.SaveChanges();

            var players = new List<Player>
            {
                new Player(){Id = playerDave, Name = "Big Boss Dave"},
                new Player(){Id = playerGrant, Name = "El Granto"},
                new Player(){Id = playerGarrett, Name = "The Openshaw"},
                new Player(){Id = playerKyle, Name = "The Slink"},
                new Player(){Id = playerJoey, Name = "Gooseman"}
            };

            players.ForEach(player => context.Players.Add(player));
            context.SaveChanges();

            var playedGames = new List<PlayedGame>();
            
            List<Player> playersInSmallWorldGame = new List<Player>()
            {
                new Player(){ Id = playerDave },
                new Player(){ Id = playerGrant }
            };

            playedGames.Add(new PlayedGame(){ GameDefinitionId = smallWorldGameDefinitionID, Players = playersInSmallWorldGame, NumberOfPlayers = playersInSmallWorldGame.Count()});

            List<Player> playersInRaceForTheGalaxyGame = new List<Player>()
            {
                new Player(){ Id = playerDave },
                new Player(){ Id = playerGarrett },
                new Player(){ Id = playerJoey}
            };

            playedGames.Add(new PlayedGame() { GameDefinitionId = raceForTheGalaxyGameDefinitionID, Players = playersInRaceForTheGalaxyGame, NumberOfPlayers = playersInRaceForTheGalaxyGame.Count() });

            List<Player> playersInSettlersOfCatanGame = new List<Player>()
            {
                new Player(){ Id = playerGarrett },
                new Player(){ Id = playerKyle },
                new Player(){ Id = playerJoey },
                new Player(){ Id = playerGrant }
            };

            playedGames.Add(new PlayedGame() { GameDefinitionId = settlersOfCatanGameDefinitionID, Players = playersInSettlersOfCatanGame, NumberOfPlayers = playersInSettlersOfCatanGame.Count() });

            playedGames.ForEach(playedGame => context.PlayedGames.Add(playedGame));
            context.SaveChanges();
        }
    }
}
