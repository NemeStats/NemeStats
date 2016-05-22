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
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;
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
                                                                  ? player.Nemesis.NemesisPlayer.Name
                                                                  : null,
                                          PreviousNemesisPlayerId = player.PreviousNemesis == null ? (int?)null : player.PreviousNemesis.NemesisPlayerId,
                                          PreviousNemesisPlayerName = player.PreviousNemesis != null && player.PreviousNemesis.NemesisPlayer != null
                                                                          ? player.PreviousNemesis.NemesisPlayer.Name
                                                                          : null,
                                          GamingGroupId = player.GamingGroupId,
                                          GamesWon =
                                              player.PlayerGameResults.Where(
                                                                             x =>
                                                                             x.PlayedGame.DatePlayed >= dateRangeFilter.FromDate &&
                                                                             x.PlayedGame.DatePlayed <= dateRangeFilter.ToDate).Count(x => x.GameRank == 1),
                                          GamesLost =
                                              player.PlayerGameResults.Where(
                                                                             x =>
                                                                             x.PlayedGame.DatePlayed >= dateRangeFilter.FromDate &&
                                                                             x.PlayedGame.DatePlayed <= dateRangeFilter.ToDate).Count(x => x.GameRank > 1),
                                          //only get championed games where this player is the current champion
                                          TotalChampionedGames =
                                              player.ChampionedGames.Count(
                                                                           champion =>
                                                                           champion.GameDefinition.ChampionId != null &&
                                                                           champion.GameDefinition.ChampionId.Value == champion.Id)
                                      }
                                     ).ToList();

            PopulateNemePointsSummary(gamingGroupId, playersWithNemesis, dateRangeFilter);
            PopulateAchivements(playersWithNemesis);

            return playersWithNemesis//--deliberately ToList() first since Linq To Entities cannot support ordering by NemePointsSummary.TotalPoints
                                      .OrderByDescending(x => x.PlayerActive)
                                      .ThenByDescending(pwn => pwn.NemePointsSummary?.TotalPoints ?? 0)
                                      .ThenByDescending(pwn => pwn.GamesWon)
                                      .ThenBy(pwn => pwn.PlayerName)
                                      .ToList();
        }

        private void PopulateAchivements(List<PlayerWithNemesis> playersWithNemesis)
        {
            var playerAchievements = dataContext.GetQueryable<PlayerAchievement>();
            if (playerAchievements != null)
            {

                foreach (var player in playersWithNemesis)
                {
                    player.AchievementsPerLevel.Add(AchievementLevel.Bronze,
                        playerAchievements.Count(
                            pa => pa.PlayerId == player.PlayerId && pa.AchievementLevel == AchievementLevel.Bronze));
                    player.AchievementsPerLevel.Add(AchievementLevel.Silver,
                        playerAchievements.Count(
                            pa => pa.PlayerId == player.PlayerId && pa.AchievementLevel == AchievementLevel.Silver));
                    player.AchievementsPerLevel.Add(AchievementLevel.Gold,
                        playerAchievements.Count(
                            pa => pa.PlayerId == player.PlayerId && pa.AchievementLevel == AchievementLevel.Gold));
                }
            }
        }


        internal virtual void PopulateNemePointsSummary(int gamingGroupId, List<PlayerWithNemesis> playersWithNemesis, IDateRangeFilter dateRangeFilter)
        {
            var nemePointsDictionary = (from playerGameResult in dataContext.GetQueryable<PlayerGameResult>()
                                        where playerGameResult.PlayedGame.GamingGroupId == gamingGroupId
                                        && playerGameResult.PlayedGame.DatePlayed >= dateRangeFilter.FromDate
                                        && playerGameResult.PlayedGame.DatePlayed <= dateRangeFilter.ToDate
                                        group playerGameResult by playerGameResult.PlayerId
                                        into groupedResults
                                        select
                                            new
                                            {
                                                BasePoints = groupedResults.Sum(x => x.NemeStatsPointsAwarded),
                                                GameDurationBonusPoints = groupedResults.Sum(x => x.GameDurationBonusPoints),
                                                WeightBonusPoints = groupedResults.Sum(x => x.GameWeightBonusPoints),
                                                PlayerId = groupedResults.Key
                                            }).ToDictionary(key => key.PlayerId, value => new NemePointsSummary(value.BasePoints, value.GameDurationBonusPoints, value.WeightBonusPoints));

            foreach (var player in playersWithNemesis)
            {
                player.NemePointsSummary = nemePointsDictionary.ContainsKey(player.PlayerId) ? nemePointsDictionary[player.PlayerId] : new NemePointsSummary(0, 0, 0);
            }
        }

        public virtual PlayerDetails GetPlayerDetails(int playerId, int numberOfRecentGamesToRetrieve)
        {
            var returnPlayer = dataContext.GetQueryable<Player>()
                                             .Include(player => player.Nemesis)
                                             .Include(player => player.Nemesis.NemesisPlayer)
                                             .Include(player => player.PreviousNemesis)
                                             .Include(player => player.PreviousNemesis.NemesisPlayer)
                                             .Include(player => player.GamingGroup)
                                             .Include(player => player.PlayerAchievements)
                                             .SingleOrDefault(player => player.Id == playerId);

            ValidatePlayerWasFound(playerId, returnPlayer);

            var playerStatistics = GetPlayerStatistics(playerId);

            var playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerId, numberOfRecentGamesToRetrieve);

            var minions = GetMinions(returnPlayer.Id);

            var playerGameSummaries = playerRepository.GetPlayerGameSummaries(playerId);

            var championedGames = GetChampionedGames(returnPlayer.Id);

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
                LongestWinningStreak = longestWinningStreak,
                NemePointsSummary = playerStatistics.NemePointsSummary,
                Achievements = returnPlayer.PlayerAchievements?.ToList() ?? new List<PlayerAchievement>()
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
            var playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
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

            playerStatistics.NemePointsSummary = GetNemePointsSummary(playerId);

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

        internal virtual NemePointsSummary GetNemePointsSummary(int playerId)
        {
            var nemePointsSummary = dataContext.GetQueryable<PlayerGameResult>()
                                          .Where(result => result.PlayerId == playerId)
                                          .GroupBy(x => x.PlayerId)
                                          .Select(
                                                  g =>
                                                  new NemePointsSummary
                                                  {
                                                      //had to cast to handle the case where there is no data:
                                                      //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
                                                      BaseNemePoints = g.Sum(x => (int?)x.NemeStatsPointsAwarded) ?? 0,
                                                      GameDurationBonusNemePoints = g.Sum(x => (int?)x.GameDurationBonusPoints) ?? 0,
                                                      WeightBonusNemePoints = g.Sum(x => (int?)x.GameWeightBonusPoints) ?? 0
                                                  })
                                .SingleOrDefault();
            return nemePointsSummary ?? new NemePointsSummary(0, 0, 0);
        }

        public virtual int GetPlayerIdForCurrentUser(string applicationUserId, int gamingGroupId)
        {
            return (from player in dataContext.GetQueryable<Player>()
                    where player.GamingGroupId == gamingGroupId
                     && player.ApplicationUserId == applicationUserId
                    select player.Id)
                    .FirstOrDefault();
        }

        public virtual PlayerQuickStats GetPlayerQuickStatsForUser(string applicationUserId, int gamingGroupId)
        {
            var playerIdForCurrentUser = GetPlayerIdForCurrentUser(applicationUserId, gamingGroupId);

            var returnValue = new PlayerQuickStats();

            if (playerIdForCurrentUser != 0)
            {
                returnValue.PlayerId = playerIdForCurrentUser;
                returnValue.NemePointsSummary = GetNemePointsSummary(playerIdForCurrentUser);

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
