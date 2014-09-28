using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Logic.Players
{
    public class PlayerRetriever : BusinessLogic.Logic.Players.IPlayerRetriever
    {
        private IDataContext dataContext;

        public PlayerRetriever(DataAccess.IDataContext dataContext)
        {
            this.dataContext = dataContext;
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

        public List<Player> GetAllPlayersWithNemesisInfo(int gamingGroupId)
        {
            return GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                                        .Include(player => player.Nemesis)
                                        .Include(player => player.Nemesis.NemesisPlayer)
                                        .OrderBy(player => player.Name)
                                        .ToList();
        }

        public virtual PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve)
        {
            Player returnPlayer = dataContext.FindById<Player>(playerID);

            PlayerStatistics playerStatistics = GetPlayerStatistics(playerID);

            List<PlayerGameResult> playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerID, numberOfRecentGamesToRetrieve);

            Nemesis nemesis = RetrieveNemesis(returnPlayer.NemesisId);

            List<Player> minions = GetMinions(returnPlayer.Id);

            PlayerDetails playerDetails = new PlayerDetails()
            {
                Active = returnPlayer.Active,
                Id = returnPlayer.Id,
                Name = returnPlayer.Name,
                GamingGroupId = returnPlayer.GamingGroupId,
                PlayerGameResults = playerGameResults,
                PlayerStats = playerStatistics,
                PlayerNemesis = nemesis,
                Minions = minions
            };

            return playerDetails;
        }

        internal virtual List<Player> GetMinions(int nemesisPlayerId)
        {
            return (from Player player in dataContext.GetQueryable<Player>().Include(p => p.Nemesis)
                     where player.Nemesis.NemesisPlayerId == nemesisPlayerId
                        select player).ToList();
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

        private Nemesis RetrieveNemesis(int? nemesisId)
        {
            Nemesis nemesis;
            if (nemesisId.HasValue)
            {
                nemesis = dataContext.FindById<Nemesis>(nemesisId.Value);
                nemesis.NemesisPlayer = dataContext.FindById<Player>(nemesis.NemesisPlayerId);
            }
            else
            {
                nemesis = new NullNemesis();
            }
            return nemesis;
        }
    }
}
