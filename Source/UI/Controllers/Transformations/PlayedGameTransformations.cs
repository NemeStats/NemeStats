using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Controllers.Transformations
{
    public class PlayedGameTransformations
    {
        //TODO Tosho says to use Automapper instead of doing this manually.
        public static NewlyCompletedGame MakeNewlyCompletedGame(int gameDefinitionId, ICollection<PlayerGameResult> playerGameResults)
        {
            if(playerGameResults == null)
            {
                throw new ArgumentException("playerGameResults cannot be null");
            }
            
            return new NewlyCompletedGame()
            {
                GameDefinitionId = gameDefinitionId,
                
                PlayerRanks = playerGameResults.Select(x => new PlayerRank()
                {
                    PlayerId = x.PlayerId,
                    GameRank = x.GameRank
                }).ToList()
            };
        }
    }
}