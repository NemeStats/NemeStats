using System.Data.Entity;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionRetriever : IGameDefinitionRetriever
    {
        protected IDataContext dataContext;

        public GameDefinitionRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual IList<GameDefinitionSummary> GetAllGameDefinitions(int gamingGroupId)
        {
            return (from gameDefinition in dataContext.GetQueryable<GameDefinition>().Include(game => game.PlayedGames)
                where gameDefinition.GamingGroupId == gamingGroupId
                        && gameDefinition.Active
                select new GameDefinitionSummary
                {
                    Active = gameDefinition.Active,
                    BoardGameGeekObjectId = gameDefinition.BoardGameGeekObjectId,
                    Name = gameDefinition.Name,
                    Description = gameDefinition.Description,
                    GamingGroupId = gameDefinition.GamingGroupId,
                    Id = gameDefinition.Id,
                    PlayedGames = gameDefinition.PlayedGames,
                    TotalNumberOfGamesPlayed = gameDefinition.PlayedGames.Count
                })
                .OrderBy(game => game.Name)
                .ToList();
        }

        public virtual GameDefinitionSummary GetGameDefinitionDetails(int id, int numberOfPlayedGamesToRetrieve)
        {
            GameDefinitionSummary gameDefinitionSummary = (from gameDefinition in dataContext.GetQueryable<GameDefinition>()
                                                                                             .Include(game => game.PlayedGames)
                                                                                             .Include(game => game.GamingGroup)
                                                           where gameDefinition.Id == id
                                                           select new GameDefinitionSummary
                                                           {
                                                               Active = gameDefinition.Active,
                                                               BoardGameGeekObjectId = gameDefinition.BoardGameGeekObjectId,
                                                               Name = gameDefinition.Name,
                                                               Description = gameDefinition.Description,
                                                               GamingGroup = gameDefinition.GamingGroup,
                                                               GamingGroupId = gameDefinition.GamingGroupId,
                                                               GamingGroupName = gameDefinition.GamingGroup.Name,
                                                               Id = gameDefinition.Id,
                                                               TotalNumberOfGamesPlayed = gameDefinition.PlayedGames.Count,
                                                               GameDefinition = gameDefinition
                                                           }).First();

            IList<PlayedGame> playedGames = AddPlayedGamesToTheGameDefinition(numberOfPlayedGamesToRetrieve, gameDefinitionSummary);
            IList<int> distinctPlayerIds = AddPlayerGameResultsToEachPlayedGame(playedGames);
            AddPlayersToPlayerGameResults(playedGames, distinctPlayerIds);

            return gameDefinitionSummary;
        }

        private IList<PlayedGame> AddPlayedGamesToTheGameDefinition(
            int numberOfPlayedGamesToRetrieve,
            GameDefinitionSummary gameDefinitionSummary)
        {
            IList<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>().Include(playedGame => playedGame.PlayerGameResults)
                .Where(playedGame => playedGame.GameDefinitionId == gameDefinitionSummary.Id)
                .OrderByDescending(playedGame => playedGame.DatePlayed)
                .Take(numberOfPlayedGamesToRetrieve)
                .ToList();

            //TODO this is very hacky as I had to add GameDefinition as an internal property on GameDefinitionSummary just so I could set it here. 
            //Need to revisit this when my brain is less foggy. Or someone with a less foggy brain in general needs to take a peak.
            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.GameDefinition = gameDefinitionSummary.GameDefinition;
            }

            gameDefinitionSummary.PlayedGames = playedGames;

            return playedGames;
        }

        private IList<int> AddPlayerGameResultsToEachPlayedGame(IList<PlayedGame> playedGames)
        {
            List<int> playedGameIds = (from playedGame in playedGames
                                       select playedGame.Id).ToList();

            IList<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
                .Where(playerGameResult => playedGameIds.Contains(playerGameResult.PlayedGameId))
                .OrderBy(playerGameResult => playerGameResult.GameRank)
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
