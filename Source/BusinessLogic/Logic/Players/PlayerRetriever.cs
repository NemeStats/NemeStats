#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models.Utility;

namespace BusinessLogic.Logic.Players
{
    public class PlayerRetriever : IPlayerRetriever
    {
        private readonly IDataContext dataContext;
        private readonly IPlayerRepository playerRepository;
        private readonly IPlayedGameRetriever playedGameRetriever;
        public const string EXCEPTION_MESSAGE_PLAYER_COULD_NOT_BE_FOUND = "Could not find player with Id: {0}";

        public PlayerRetriever(IDataContext dataContext, IPlayerRepository playerRepository, IPlayedGameRetriever playedGameRetriever)
        {
            this.dataContext = dataContext;
            this.playerRepository = playerRepository;
            this.playedGameRetriever = playedGameRetriever;
        }

        internal IQueryable<Player> GetAllPlayersInGamingGroupQueryable(int gamingGroupId)
        {
            return dataContext.GetQueryable<Player>()
                .Where(player => player.GamingGroupId == gamingGroupId);
        }

        public List<Player> GetAllPlayers(int gamingGroupId, bool includeInactive = true)
        {
            return GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                .Where(x => x.Active || x.Active != includeInactive)
                .OrderByDescending(player => player.Active)
                .ThenBy(player => player.Name)
                .ToList();
        }

        public List<PlayerWithNemesis> GetAllPlayersWithNemesisInfo(int gamingGroupId, IDateRangeFilter dateRangeFilter = null)
        {
            if (dateRangeFilter == null)
            {
                dateRangeFilter = new BasicDateRangeFilter();
            }

            var playersWithNemesis = (from Player player in GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                .Include(player => player.PlayerGameResults)
                                      select new PlayerWithNemesis
                                      {
                                          PlayerId = player.Id,
                                          ApplicationUserId = player.ApplicationUserId,
                                          PlayerName = player.Name,
                                          PlayerActive = player.Active,
                                          PlayerRegistered = !string.IsNullOrEmpty(player.ApplicationUserId),
                                          NemesisPlayerId = player.Nemesis == null ? (int?)null : player.Nemesis.NemesisPlayerId,
                                          NemesisPlayerName = player.Nemesis != null && player.Nemesis.NemesisPlayer != null
                                              ? player.Nemesis.NemesisPlayer.Name : null,
                                          PreviousNemesisPlayerId = player.PreviousNemesis == null ? (int?)null : player.PreviousNemesis.NemesisPlayerId,
                                          PreviousNemesisPlayerName = player.PreviousNemesis != null && player.PreviousNemesis.NemesisPlayer != null
                                              ? player.PreviousNemesis.NemesisPlayer.Name : null,
                                          GamingGroupId = player.GamingGroupId,
                                          GamesWon = player.PlayerGameResults.Where(x => x.PlayedGame.DatePlayed >= dateRangeFilter.FromDate && x.PlayedGame.DatePlayed <= dateRangeFilter.ToDate).Count(x => x.GameRank == 1),
                                          GamesLost = player.PlayerGameResults.Where(x => x.PlayedGame.DatePlayed >= dateRangeFilter.FromDate && x.PlayedGame.DatePlayed <= dateRangeFilter.ToDate).Count(x => x.GameRank > 1),
                                          TotalPoints = player.PlayerGameResults.Where(x => x.PlayedGame.DatePlayed >= dateRangeFilter.FromDate && x.PlayedGame.DatePlayed <= dateRangeFilter.ToDate)
                                              .Select(pgr => pgr.TotalPoints)
                                              .DefaultIfEmpty(0)
                                              .Sum(),
                                          //only get championed games where this player is the current champion
                                          TotalChampionedGames = player.ChampionedGames.Count(champion => champion.GameDefinition.ChampionId != null && champion.GameDefinition.ChampionId.Value == champion.Id)
                                      }
                ).OrderByDescending(x => x.PlayerActive).ThenByDescending(pwn => pwn.TotalPoints).ThenByDescending(pwn => pwn.GamesWon).ThenBy(pwn => pwn.PlayerName)
                .ToList();

            return playersWithNemesis;
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

            IList<PlayerGameSummary> playerGameSummaries = playerRepository.GetPlayerGameSummaries(playerId);

            List<Champion> championedGames = GetChampionedGames(returnPlayer.Id);

            var formerChampionedGames = GetFormerChampionedGames(returnPlayer.Id);

            var longestWinningStreak = playerRepository.GetLongestWinningStreak(playerId);

            var playerDetails = new PlayerDetails()
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
                ChampionedGames = championedGames,
                PlayerVersusPlayersStatistics = playerRepository.GetPlayerVersusPlayersStatistics(playerId),
                FormerChampionedGames = formerChampionedGames,
                LongestWinningStreak = longestWinningStreak
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

        internal virtual List<GameDefinition> GetFormerChampionedGames(int playerId)
        {
            return
                (from Champion champion in
                     dataContext.GetQueryable<Champion>().Include(c => c.GameDefinition)
                 where champion.PlayerId == playerId
                 select champion.GameDefinition)
                 .ToList();
        }

        internal virtual List<PlayerGameResult> GetPlayerGameResultsWithPlayedGameAndGameDefinition(
            int playerID,
            int numberOfRecentGamesToRetrieve)
        {
            List<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
                        .Where(result => result.PlayerId == playerID)
                        .OrderByDescending(result => result.PlayedGame.DatePlayed)
                        .ThenByDescending(result => result.PlayedGame.Id)
                        .Take(numberOfRecentGamesToRetrieve)
                        .Include(result => result.PlayedGame.GameDefinition.BoardGameGeekGameDefinition)
                        .ToList();
            return playerGameResults;
        }

        public virtual PlayerStatistics GetPlayerStatistics(int playerId)
        {
            var playerStatistics = new PlayerStatistics();
            var gameDefinitionTotals = GetGameDefinitionTotals(playerId);
            playerStatistics.GameDefinitionTotals = gameDefinitionTotals;

            var topLevelTotals = GetTopLevelTotals(gameDefinitionTotals);

            playerStatistics.TotalGames = topLevelTotals.TotalGames;
            playerStatistics.TotalGamesLost = topLevelTotals.TotalGamesLost;
            playerStatistics.TotalGamesWon = topLevelTotals.TotalGamesWon;

            if (playerStatistics.TotalGames > 0)
            {
                playerStatistics.WinPercentage = (int)((decimal)playerStatistics.TotalGamesWon / (playerStatistics.TotalGames) * 100);
            }

            playerStatistics.TotalPoints = GetTotalNemePoints(playerId);

            //had to cast to handle the case where there is no data:
            //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
            playerStatistics.AveragePlayersPerGame = (float?)dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.PlayerGameResults.Any(result => result.PlayerId == playerId))
                    .Average(game => (int?)game.NumberOfPlayers) ?? 0F;

            return playerStatistics;
        }

        internal virtual TopLevelTotals GetTopLevelTotals(GameDefinitionTotals gameDefinitionTotals)
        {
            var returnResult = new TopLevelTotals
            {
                TotalGamesLost = gameDefinitionTotals.SummariesOfGameDefinitionTotals.Sum(x => x.GamesLost),
                TotalGamesWon = gameDefinitionTotals.SummariesOfGameDefinitionTotals.Sum(x => x.GamesWon)
            };
            returnResult.TotalGames = returnResult.TotalGamesLost + returnResult.TotalGamesWon;

            return returnResult;
        }

        internal virtual GameDefinitionTotals GetGameDefinitionTotals(int playerId)
        {
            var playerGameSummaries = playerRepository.GetPlayerGameSummaries(playerId);
            var gameDefinitionTotals = new GameDefinitionTotals
            {
                SummariesOfGameDefinitionTotals = playerGameSummaries.Select(playerGameSummary => new GameDefinitionTotal
                {
                    GameDefinitionId = playerGameSummary.GameDefinitionId,
                    GameDefinitionName = playerGameSummary.GameDefinitionName,
                    GamesLost = playerGameSummary.NumberOfGamesLost,
                    GamesWon = playerGameSummary.NumberOfGamesWon
                }).ToList()
            };
            return gameDefinitionTotals;
        }

        internal class TopLevelTotals
        {
            public int TotalGames { get; internal set; }
            public int TotalGamesLost { get; internal set; }
            public int TotalGamesWon { get; internal set; }
        }

        internal virtual int GetTotalNemePoints(int playerId)
        {
            int? totalPoints = dataContext.GetQueryable<PlayerGameResult>()
                            .Where(result => result.PlayerId == playerId)
                            //had to cast to handle the case where there is no data:
                            //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
                            .Sum(playerGameResults => (int?)playerGameResults.TotalPoints) ?? 0;
            if (totalPoints.HasValue)
            {
                return totalPoints.Value;
            }
            else
            {
                return 0;
            }
        }

        public virtual PlayerQuickStats GetPlayerQuickStatsForUser(string applicationUserId, int gamingGroupId)
        {
            var q = dataContext.GetQueryable<Player>().ToList();
            int playerIdForCurrentUser = (from player in dataContext.GetQueryable<Player>()
                                          where player.GamingGroupId == gamingGroupId
                                           && player.ApplicationUserId == applicationUserId
                                          select player.Id)
                                          .FirstOrDefault();

            var returnValue = new PlayerQuickStats();

            if (playerIdForCurrentUser != 0)
            {
                returnValue.PlayerId = playerIdForCurrentUser;
                returnValue.TotalPoints = GetTotalNemePoints(playerIdForCurrentUser);

                var gameDefinitionTotals = GetGameDefinitionTotals(playerIdForCurrentUser);
                var topLevelTotals = GetTopLevelTotals(gameDefinitionTotals);
                returnValue.TotalGamesPlayed = topLevelTotals.TotalGames;
                returnValue.TotalGamesWon = topLevelTotals.TotalGamesWon;

                var lastPlayedGameForGamingGroupList = playedGameRetriever.GetRecentGames(1, gamingGroupId);
                if (lastPlayedGameForGamingGroupList.Count() == 1)
                {
                    var lastGame = lastPlayedGameForGamingGroupList[0];
                    returnValue.LastGamingGroupGame = new PlayedGameQuickStats
                    {
                        DatePlayed = lastGame.DatePlayed,
                        GameDefinitionName = lastGame.GameDefinition.Name,
                        GameDefinitionId = lastGame.GameDefinitionId,
                        PlayedGameId = lastGame.Id,
                        WinnerType = lastGame.WinnerType
                    };

                    if (lastGame.WinningPlayer != null)
                    {
                        returnValue.LastGamingGroupGame.WinningPlayerId = lastGame.WinningPlayer.Id;
                        returnValue.LastGamingGroupGame.WinningPlayerName = lastGame.WinningPlayer.Name;
                    }

                    var bggGameDefinition = lastGame.GameDefinition.BoardGameGeekGameDefinition;

                    if (bggGameDefinition != null)
                    {
                        returnValue.LastGamingGroupGame.BoardGameGeekUri = BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(bggGameDefinition.Id);
                        returnValue.LastGamingGroupGame.ThumbnailImageUrl = bggGameDefinition.Thumbnail;
                    }
                }
            }

            return returnValue;
        }
    }
}
