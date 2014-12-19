using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Logic.Players
{
    public class PlayerRetriever : IPlayerRetriever
    {
        private readonly IDataContext dataContext;
        private readonly IPlayerRepository playerRepository;
        public const string EXCEPTION_MESSAGE_PLAYER_COULD_NOT_BE_FOUND = "Could not find player with Id: {0}";

        public PlayerRetriever(IDataContext dataContext, IPlayerRepository playerRepository)
        {
            this.dataContext = dataContext;
            this.playerRepository = playerRepository;
        }

        internal IQueryable<Player> GetAllPlayersInGamingGroupQueryable(int gamingGroupId)
        {
            return dataContext.GetQueryable<Player>().Where(
               player => player.GamingGroupId == gamingGroupId
                   && player.Active);
        }

        public List<Player> GetAllPlayers(int gamingGroupId)
        {
            return GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                .OrderBy(player => player.Name)
                .ToList();
        }

        public List<PlayerWithNemesis> GetAllPlayersWithNemesisInfo(int gamingGroupId)
        {
            return (from Player player in GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                        select new PlayerWithNemesis
                        {
                           PlayerId = player.Id,
                           PlayerName = player.Name,
                           PlayerRegistered = !string.IsNullOrEmpty(player.ApplicationUserId),
                           NemesisPlayerId = player.Nemesis == null ? (int?)null : player.Nemesis.NemesisPlayerId,
                           NemesisPlayerName = player.Nemesis != null && player.Nemesis.NemesisPlayer != null 
                            ? player.Nemesis.NemesisPlayer.Name : null,
                           PreviousNemesisPlayerId = player.PreviousNemesis == null ? (int?)null : player.PreviousNemesis.NemesisPlayerId,
                           PreviousNemesisPlayerName = player.PreviousNemesis != null && player.PreviousNemesis.NemesisPlayer != null 
                            ? player.PreviousNemesis.NemesisPlayer.Name : null,
                            GamingGroupId = player.GamingGroupId
                        }
                   ).ToList();
            //GetAllPlayersInGamingGroupQueryable(gamingGroupId)
            //                        .Include(player => player.Nemesis)
            //                        .Include(player => player.Nemesis.NemesisPlayer)
            //                        .Include(player => player.PreviousNemesis)
            //                        .Include(player => player.PreviousNemesis.NemesisPlayer)
            //                        .OrderBy(player => player.Name)
            //                        .ToList();
        }

        public virtual PlayerDetails GetPlayerDetails(int playerId, int numberOfRecentGamesToRetrieve)
        {
            Player returnPlayer = dataContext.GetQueryable<Player>()
                                             .Include(player => player.Nemesis)
                                             .Include(player => player.Nemesis.NemesisPlayer)
                                             .Include(player => player.PreviousNemesis)
                                             .Include(player => player.PreviousNemesis.NemesisPlayer)
                                             .Include(player => player.GamingGroup)
                                             .SingleOrDefault(player => player.Id == playerId);

            ValidatePlayerWasFound(playerId, returnPlayer);

            PlayerStatistics playerStatistics = GetPlayerStatistics(playerId);

            List<PlayerGameResult> playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerId, numberOfRecentGamesToRetrieve);

            List<Player> minions = GetMinions(returnPlayer.Id);

            List<PlayerGameSummary> playerGameSummaries = playerRepository.GetPlayerGameSummaries(playerId);

            List<Champion> championedGames = GetChampionedGames(returnPlayer.Id);

            PlayerDetails playerDetails = new PlayerDetails()
            {
                Active = returnPlayer.Active,
                ApplicationUserId = returnPlayer.ApplicationUserId,
                Id = returnPlayer.Id,
                Name = returnPlayer.Name,
                GamingGroupId = returnPlayer.GamingGroupId,
                GamingGroupName = returnPlayer.GamingGroup.Name,
                PlayerGameResults = playerGameResults,
                PlayerStats = playerStatistics,
                CurrentNemesis = returnPlayer.Nemesis ?? new NullNemesis(),
                PreviousNemesis = returnPlayer.PreviousNemesis ?? new NullNemesis(),
                Minions = minions,
                PlayerGameSummaries = playerGameSummaries,
                ChampionedGames = championedGames
            };

            return playerDetails;
        }

        private static void ValidatePlayerWasFound(int playerId, Player returnPlayer)
        {
            if (returnPlayer == null)
            {
                throw new KeyNotFoundException(string.Format(EXCEPTION_MESSAGE_PLAYER_COULD_NOT_BE_FOUND, playerId));
            }
        }

        internal virtual List<Player> GetMinions(int nemesisPlayerId)
        {
            return (from Player player in dataContext.GetQueryable<Player>().Include(p => p.Nemesis)
                     where player.Nemesis.NemesisPlayerId == nemesisPlayerId
                        select player).ToList();
        }

        internal virtual List<Champion> GetChampionedGames(int playerId)
        {
            return
                (from GameDefinition gameDefinition in
                     dataContext.GetQueryable<GameDefinition>().Include(g => g.Champion)
                 where gameDefinition.Champion.PlayerId == playerId
                 select gameDefinition.Champion).Include(c => c.GameDefinition)
                 .ToList();
        }

        internal virtual List<PlayerGameResult> GetPlayerGameResultsWithPlayedGameAndGameDefinition(
            int playerID,
            int numberOfRecentGamesToRetrieve)
        {
            List<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
                        .Where(result => result.PlayerId == playerID)
                        .OrderByDescending(result => result.PlayedGame.DatePlayed)
                        .Take(numberOfRecentGamesToRetrieve)
                        .Include(result => result.PlayedGame)
                        .Include(result => result.PlayedGame.GameDefinition)
                        .ToList();
            return playerGameResults;
        }

        public virtual PlayerStatistics GetPlayerStatistics(int playerId)
        {
            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerStatistics.TotalGames = dataContext.GetQueryable<PlayerGameResult>()
                .Count(playerGameResults => playerGameResults.PlayerId == playerId);

            int? totalPoints = dataContext.GetQueryable<PlayerGameResult>()
                .Where(result => result.PlayerId == playerId)
                //had to cast to handle the case where there is no data:
                //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
                .Sum(playerGameResults => (int?)playerGameResults.GordonPoints) ?? 0;

            if (totalPoints.HasValue)
            {
                playerStatistics.TotalPoints = totalPoints.Value;
            }

            //had to cast to handle the case where there is no data:
            //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
            playerStatistics.AveragePlayersPerGame = (float?)dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.PlayerGameResults.Any(result => result.PlayerId == playerId))
                    .Average(game => (int?)game.NumberOfPlayers) ?? 0F;

            return playerStatistics;
        }
    }
}
