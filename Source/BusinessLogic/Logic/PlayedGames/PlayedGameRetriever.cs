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
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models.Utility;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameRetriever : IPlayedGameRetriever
    {
        private readonly IDataContext dataContext;

        public PlayedGameRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<PlayedGame> GetRecentGames(int numberOfGames, int gamingGroupId, IDateRangeFilter dateRangeFilter = null)
        {
            if(dateRangeFilter == null)
            {
                dateRangeFilter = new BasicDateRangeFilter();
            }

            List<PlayedGame> playedGames = dataContext.GetQueryable<PlayedGame>()
                .Where(game => game.GamingGroupId == gamingGroupId
                            && game.DatePlayed >= dateRangeFilter.FromDate
                                              && game.DatePlayed <= dateRangeFilter.ToDate)
                .Include(playedGame => playedGame.GameDefinition.BoardGameGeekGameDefinition)
                .Include(playedGame => playedGame.GamingGroup)
                .Include(playedGame => playedGame.PlayerGameResults
                    .Select(playerGameResult => playerGameResult.Player))
                    .OrderByDescending(orderBy => orderBy.DatePlayed)
                    .ThenByDescending(orderBy => orderBy.DateCreated)
                .Take(numberOfGames)
                .ToList();

            //TODO this seems ridiculous but I can't see how to order a related entity in Entity Framework :(
            foreach (PlayedGame playedGame in playedGames)
            {
                playedGame.PlayerGameResults = playedGame.PlayerGameResults.OrderBy(orderBy => orderBy.GameRank).ToList();
            }

            return playedGames;
        }


        public PlayedGame GetPlayedGameDetails(int playedGameId)
        {
            PlayedGame result = dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.Id == playedGameId)
                    .Include(playedGame => playedGame.GameDefinition)
                    .Include(playedGame => playedGame.GameDefinition.BoardGameGeekGameDefinition)
                    .Include(playedGame => playedGame.GamingGroup)
                    .Include(playedGame => playedGame.PlayerGameResults
                        .Select(playerGameResult => playerGameResult.Player))
                    .FirstOrDefault();

            if (result == null)
            {
                throw new EntityDoesNotExistException(typeof(PlayedGame), playedGameId);
            }

            result.PlayerGameResults = result.PlayerGameResults.OrderBy(playerGameResult => playerGameResult.GameRank).ToList();

            return result;
        }

        public List<PublicGameSummary> GetRecentPublicGames(int numberOfGames)
        {
            var publicGameSummaries = (from playedGame in dataContext.GetQueryable<PlayedGame>()
                 group new PublicGameSummary
                 {
                     PlayedGameId = playedGame.Id,
                     GameDefinitionId = playedGame.GameDefinitionId,
                     GameDefinitionName = playedGame.GameDefinition.Name,
                     GamingGroupId = playedGame.GamingGroupId,
                     GamingGroupName = playedGame.GamingGroup.Name,
                     WinnerType = (playedGame.PlayerGameResults.All(x => x.GameRank == 1) ? WinnerTypes.TeamWin :
                                              playedGame.PlayerGameResults.All(x => x.GameRank != 1) ? WinnerTypes.TeamLoss :
                                                  WinnerTypes.PlayerWin),
                     WinningPlayer = playedGame.PlayerGameResults.FirstOrDefault(player => player.GameRank == 1).Player,
                     DatePlayed = playedGame.DatePlayed,
                     ThumbnailImageUrl = playedGame.GameDefinition.BoardGameGeekGameDefinition == null
                                              ? null : playedGame.GameDefinition.BoardGameGeekGameDefinition.Thumbnail,
                     BoardGameGeekObjectId = playedGame.GameDefinition.BoardGameGeekGameDefinitionId
                 } 
                 by playedGame.GamingGroupId
                 into gamingGroups
                 select gamingGroups
                    .OrderByDescending(x => x.DatePlayed)
                    .ThenByDescending(y => y.PlayedGameId)
                    .FirstOrDefault()
                )
                    .OrderByDescending(x => x.DatePlayed)
                    .ThenByDescending(y => y.PlayedGameId)
                    .Take(numberOfGames)
                    .ToList();

            foreach (var publicGameSummary in publicGameSummaries)
            {
                publicGameSummary.BoardGameGeekUri =
                    BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(publicGameSummary.BoardGameGeekObjectId);
            }
            return publicGameSummaries;
        }

        public List<PlayedGameSearchResult> SearchPlayedGames(PlayedGameFilter playedGameFilter)
        {
            var queryable = (from playedGame in dataContext.GetQueryable<PlayedGame>()
                                                           .OrderByDescending(game => game.DatePlayed)
                                                           .ThenByDescending(game => game.DateCreated)
                             select new PlayedGameSearchResult
                             {
                                 PlayedGameId = playedGame.Id,
                                 GameDefinitionId = playedGame.GameDefinitionId,
                                 GameDefinitionName = playedGame.GameDefinition.Name,
                                 BoardGameGeekGameDefinitionId = playedGame.GameDefinition.BoardGameGeekGameDefinitionId,
                                 GamingGroupId = playedGame.GamingGroupId,
                                 GamingGroupName = playedGame.GamingGroup.Name,
                                 Notes = playedGame.Notes,
                                 DatePlayed = playedGame.DatePlayed,
                                 DateLastUpdated = playedGame.DateCreated,
                                 PlayerGameResults = playedGame.PlayerGameResults.Select(x => new PlayerResult
                                 {
                                     GameRank = x.GameRank,
                                     NemeStatsPointsAwarded = x.NemeStatsPointsAwarded,
                                     PlayerId = x.PlayerId,
                                     PlayerName = x.Player.Name,
                                     PlayerActive = x.Player.Active,
                                     PointsScored = x.PointsScored,
                                     DatePlayed = x.PlayedGame.DatePlayed,
                                     GameDefinitionId = x.PlayedGame.GameDefinitionId,
                                     GameName = x.PlayedGame.GameDefinition.Name,
                                     PlayedGameId = x.PlayedGameId
                                 }).ToList()
                             });


            queryable = AddSearchCriteria(playedGameFilter, queryable);

            var results = queryable.ToList();

            SortPlayerResultsWithinEachSearchResult(results);

            return results;
        }

        private static void SortPlayerResultsWithinEachSearchResult(List<PlayedGameSearchResult> results)
        {
            foreach (var playedGameSearchResults in results)
            {
                playedGameSearchResults.PlayerGameResults = playedGameSearchResults.PlayerGameResults.OrderBy(x => x.GameRank).ToList();
            }
        }

        private static IQueryable<PlayedGameSearchResult> AddSearchCriteria(PlayedGameFilter playedGameFilter, IQueryable<PlayedGameSearchResult> queryable)
        {
            if (playedGameFilter.GamingGroupId.HasValue)
            {
                queryable = queryable.Where(query => query.GamingGroupId == playedGameFilter.GamingGroupId.Value);
            }

            if (playedGameFilter.GameDefinitionId.HasValue)
            {
                queryable = queryable.Where(query => query.GameDefinitionId == playedGameFilter.GameDefinitionId.Value);
            }

            if (!string.IsNullOrEmpty(playedGameFilter.StartDateGameLastUpdated))
            {
                var startDate = ParseDateTime(playedGameFilter.StartDateGameLastUpdated);
                queryable = queryable.Where(query => DbFunctions.TruncateTime(query.DateLastUpdated) >= startDate);
            }

            if (!string.IsNullOrEmpty(playedGameFilter.EndDateGameLastUpdated))
            {
                var endDate = ParseDateTime(playedGameFilter.EndDateGameLastUpdated);
                queryable = queryable.Where(query => DbFunctions.TruncateTime(query.DateLastUpdated) <= endDate);
            }

            if (playedGameFilter.PlayerId.HasValue)
            {
                queryable = queryable.Where(query => query.PlayerGameResults.Any(x => x.PlayerId == playedGameFilter.PlayerId));
            }

            if (playedGameFilter.MaximumNumberOfResults.HasValue)
            {
                queryable = queryable.Take(playedGameFilter.MaximumNumberOfResults.Value);
            }
            return queryable;
        }

        private static DateTime ParseDateTime(string inputDate)
        {
            try
            {
                return DateTime.ParseExact(inputDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                throw new InvalidDateFormatException(inputDate);
            }
        }
    }
}
