using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class CompletedGame
    {
        NerdScorekeeperDbContext dbContext = null;

        public CompletedGame(NerdScorekeeperDbContext context)
        {
            dbContext = context;
        }

        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame)
        {
            if(newlyCompletedGame.GameDefinitionId == 0)
            {
                throw new ArgumentException("Must pass a valid GameDefinitionId.");
            }
            
            if(newlyCompletedGame.PlayerRanks.Count < 1)
            {
                throw new ArgumentException("Must pass in at least one player");
            }

            var playerList = newlyCompletedGame.PlayerRanks.Select(x =>  new Player() { Id = x.PlayerId} ).ToList();

            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId,
                NumberOfPlayers = newlyCompletedGame.PlayerRanks.Count(),
                Players = playerList
            };

            dbContext.PlayedGames.Add(playedGame);
            dbContext.SaveChanges();

            return playedGame;
        }
    }
}
