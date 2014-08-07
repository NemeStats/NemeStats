using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionRetrieverImpl : GameDefinitionRetriever
    {
        protected DataContext dataContext;

        public GameDefinitionRetrieverImpl(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IList<GameDefinition> GetAllGameDefinitions(ApplicationUser currentUser)
        {
            return dataContext.GetQueryable<GameDefinition>(currentUser)
                .Where(gameDefinition => gameDefinition.GamingGroupId == currentUser.CurrentGamingGroupId.Value)
                .ToList();
        }

        public GameDefinition GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve, ApplicationUser currentUser)
        {
            GameDefinition gameDefinition = dataContext.GetQueryable<GameDefinition>(currentUser)
                .Where(gameDef => gameDef.Id == id)
                .FirstOrDefault();

            IList<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>(currentUser)
                .Where(playedGame => playedGame.GameDefinitionId == gameDefinition.Id)
                .Take(numberOfPlayedGamesToRetrieve)
                .ToList();
            gameDefinition.PlayedGames = playedGames;

            List<int> playedGameIds = (from playedGame in playedGames
                                       select playedGame.Id).ToList();

            IList<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>(currentUser)
                .Where(playerGameResult => playedGameIds.Contains(playerGameResult.PlayedGameId))
                .ToList();

            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = (from playerGameResult in playerGameResults
                                                where playerGameResult.PlayedGameId == playedGame.Id
                                                select playerGameResult).ToList();
            }

            //TODO implement validation
            return gameDefinition;
        }
    }
}
