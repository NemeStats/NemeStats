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

        protected override void Seed(NerdScorekeeperDbContext context)
        {
            var gameDefinitions = new List<GameDefinition>
            {
                new GameDefinition(){ID = smallWorldGameDefinitionID, Name = "Small World", Description="Dominate the small world."},
                new GameDefinition(){ID = raceForTheGalaxyGameDefinitionID, Name = "Race For The Galaxy", Description="Win the race for the galaxy."},
                new GameDefinition(){ID = settlersOfCatanGameDefinitionID, Name = "Settlers of Catan", Description="Go settle Catan."}
            };

            gameDefinitions.ForEach(game => context.GameDefinitions.Add(game));
            context.SaveChanges();

            var players = new List<Player>
            {
                new Player(){ID = 10, Name = "Big Boss Dave"},
                new Player(){ID = 11, Name = "El Granto"},
                new Player(){ID = 12, Name = "The Openshaw"},
                new Player(){ID = 13, Name = "The Slink"},
                new Player(){ID = 14, Name = "Gooseman"}
            };

            players.ForEach(player => context.Players.Add(player));
            context.SaveChanges();

            var playedGames = new List<PlayedGame>();
            
            List<Player> playersInSmallWorldGame = new List<Player>()
            {
                new Player(){ ID = 10 },
                new Player(){ ID = 11 }
            };

            playedGames.Add(new PlayedGame(){ GameDefinitionID = smallWorldGameDefinitionID, Players = playersInSmallWorldGame, NumberOfPlayers = playersInSmallWorldGame.Count()});

            List<Player> playersInRaceForTheGalaxyGame = new List<Player>()
            {
                new Player(){ ID = 10 },
                new Player(){ ID = 13 },
                new Player(){ ID = 14}
            };

            playedGames.Add(new PlayedGame() { GameDefinitionID = raceForTheGalaxyGameDefinitionID, Players = playersInRaceForTheGalaxyGame, NumberOfPlayers = playersInRaceForTheGalaxyGame.Count() });

            List<Player> playersInSettlersOfCatanGame = new List<Player>()
            {
                new Player(){ ID = 12 },
                new Player(){ ID = 13 },
                new Player(){ ID = 14 },
                new Player(){ ID = 10 }
            };

            playedGames.Add(new PlayedGame() { GameDefinitionID = settlersOfCatanGameDefinitionID, Players = playersInSettlersOfCatanGame, NumberOfPlayers = playersInSettlersOfCatanGame.Count() });

            playedGames.ForEach(playedGame => context.PlayedGames.Add(playedGame));
            context.SaveChanges();
        }
    }
}
