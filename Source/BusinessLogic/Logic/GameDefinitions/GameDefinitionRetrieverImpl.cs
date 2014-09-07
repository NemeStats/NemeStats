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

        public virtual IList<GameDefinition> GetAllGameDefinitions(int gamingGroupId)
        {
            return dataContext.GetQueryable<GameDefinition>()
                .Where(game => game.GamingGroupId == gamingGroupId)
                .OrderBy(game => game.Name)
                .ToList();
        }

        public virtual GameDefinition GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve)
        {
            GameDefinition gameDefinition = dataContext.FindById<GameDefinition>(id);
            IList<PlayedGame> playedGames = AddPlayedGamesToTheGameDefinition(numberOfPlayedGamesToRetrieve, gameDefinition);
            IList<int> distinctPlayerIds = AddPlayerGameResultsToEachPlayedGame(playedGames);
            AddPlayersToPlayerGameResults(playedGames, distinctPlayerIds);
             
            //TODO implement validation
            return gameDefinition;
        }

        private IList<PlayedGame> AddPlayedGamesToTheGameDefinition(
            int numberOfPlayedGamesToRetrieve, 
            GameDefinition gameDefinition)
        {
            IList<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.GameDefinitionId == gameDefinition.Id)
                .OrderByDescending(playedGame => playedGame.DatePlayed)
                .Take(numberOfPlayedGamesToRetrieve)
                .ToList();
            gameDefinition.PlayedGames = playedGames;
            return playedGames;
        }

        private IList<int> AddPlayerGameResultsToEachPlayedGame(IList<PlayedGame> playedGames)
        {
            List<int> playedGameIds = (from playedGame in playedGames
                                       select playedGame.Id).ToList();

            IList<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
                .Where(playerGameResult => playedGameIds.Contains(playerGameResult.PlayedGameId))
                .ToList();

            HashSet<int> distinctPlayerIds = new HashSet<int>();

            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = (from playerGameResult in playerGameResults
                                                where playerGameResult.PlayedGameId == playedGame.Id
                                                select playerGameResult).ToList();

                ExtractDistinctListOfPlayerIds(distinctPlayerIds, playedGame);
            }

            return distinctPlayerIds.ToList();
        }

        private static void ExtractDistinctListOfPlayerIds(HashSet<int> distinctPlayerIds, PlayedGame playedGame)
        {
            foreach (PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
            {
                if (!distinctPlayerIds.Contains(playerGameResult.PlayerId))
                {
                    distinctPlayerIds.Add(playerGameResult.PlayerId);
                }
            }
        }

        private void AddPlayersToPlayerGameResults(IList<PlayedGame> playedGames, IList<int> distinctPlayerIds)
        {
            IList<Player> players = dataContext.GetQueryable<Player>()
                .Where(player => distinctPlayerIds.Contains(player.Id))
                .ToList();

            foreach (PlayedGame playedGame in playedGames)
            {
                foreach (PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
                {
                    playerGameResult.Player = players.First(player => player.Id == playerGameResult.PlayerId);
                }
            }
        }
    }
}
