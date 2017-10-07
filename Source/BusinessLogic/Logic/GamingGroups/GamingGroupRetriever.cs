#region LICENSE

// NemeStats is a free website for tracking the results of board games. Copyright (C) 2015 Jacob Gordon
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program. If
// not, see <http://www.gnu.org/licenses/>

#endregion LICENSE

using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Utility;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupRetriever : IGamingGroupRetriever
    {
        private readonly IDataContext _dataContext;

        public GamingGroupRetriever(
            IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public GamingGroup GetGamingGroupById(int gamingGroupId)
        {
            var gamingGroup = _dataContext.FindById<GamingGroup>(gamingGroupId);

            return gamingGroup;
        }

        public GamingGroupSummary GetGamingGroupDetails(GamingGroupFilter filter)
        {
            var gamingGroup = _dataContext.FindById<GamingGroup>(filter.GamingGroupId);
            var summary = new GamingGroupSummary
            {
                Id = gamingGroup.Id,
                DateCreated = gamingGroup.DateCreated,
                Name = gamingGroup.Name,
                PublicDescription = gamingGroup.PublicDescription,
                PublicGamingGroupWebsite = gamingGroup.PublicGamingGroupWebsite
            };

            return summary;
        }

        public IList<GamingGroupListItemModel> GetGamingGroupsForUser(ApplicationUser applicationUser)
        {
            return _dataContext.GetQueryable<GamingGroup>()
                              .Where(gamingGroup => gamingGroup.Active 
                                && gamingGroup.UserGamingGroups.Any(ugg => ugg.ApplicationUserId == applicationUser.Id))
                              .Select(gg => new GamingGroupListItemModel { Id = gg.Id, Name = gg.Name })
                              .ToList();
        }

        public List<TopGamingGroupSummary> GetTopGamingGroups(int numberOfTopGamingGroupsToShow)
        {
            return (from gamingGroup in _dataContext.GetQueryable<GamingGroup>()
                    where gamingGroup.Active
                    select new TopGamingGroupSummary
                    {
                        GamingGroupId = gamingGroup.Id,
                        GamingGroupName = gamingGroup.Name,
                        NumberOfGamesPlayed = gamingGroup.PlayedGames.Count,
                        NumberOfPlayers = gamingGroup.Players.Count,
                        GamingGroupChampion = gamingGroup.GamingGroupChampion,
                    }).OrderByDescending(gg => gg.NumberOfGamesPlayed)
                      .ThenByDescending(gg => gg.NumberOfPlayers)
                      .Take(numberOfTopGamingGroupsToShow)
                      .ToList();
        }

        public List<GamingGroupSitemapInfo> GetGamingGroupsSitemapInfo()
        {
            return _dataContext.GetQueryable<GamingGroup>()
                .Where(x => x.Active)
                .Select(x => new GamingGroupSitemapInfo
                {
                    GamingGroupId = x.Id,
                    DateCreated = x.DateCreated,
                    DateLastGamePlayed =
                        x.PlayedGames.OrderByDescending(playedGame => playedGame.DatePlayed).Select(playedGame => playedGame.DatePlayed).FirstOrDefault() 
                })
                .OrderBy(x => x.GamingGroupId)
                .ToList();
        }

        public GamingGroupStats GetGamingGroupStats(int gamingGroupId, BasicDateRangeFilter dateRangeFilter)
        {
            var playedGameTotals = _dataContext.GetQueryable<PlayedGame>()
                .Where(x => x.GamingGroupId == gamingGroupId 
                    && x.DatePlayed >= dateRangeFilter.FromDate
                    && x.DatePlayed <= dateRangeFilter.ToDate)
                .GroupBy(x => x.GameDefinitionId)
                .Select(g => new
                {
                    Id = g.Key,
                    NumberOfGamesPlayed = g.Count()
                }).ToList();

            var totalPlayedGames = playedGameTotals.Sum(x => x.NumberOfGamesPlayed);
            var totalNumberOfGamesWithPlays = playedGameTotals.Distinct().Count();

            var numberOfGamesOwned = _dataContext.GetQueryable<GameDefinition>().Count(x => x.GamingGroupId == gamingGroupId);

            var playerResults = _dataContext.GetQueryable<Player>()
                .Where(x => x.GamingGroupId == gamingGroupId)
                .GroupBy(x => x.PlayerGameResults.Any(y => y.PlayedGame.DatePlayed >= dateRangeFilter.FromDate
                                              && y.PlayedGame.DatePlayed <= dateRangeFilter.ToDate))
                .Select(x => new
                {
                    HasPlays = x.Key,
                    NumberOfRecords = x.Count()
                }).ToList();
            var totalNumberOfPlayersWithPlays = 0;
            var resultForPlayersWithPlays = playerResults.SingleOrDefault(x => x.HasPlays);
            if (resultForPlayersWithPlays != null)
            {
                totalNumberOfPlayersWithPlays = resultForPlayersWithPlays.NumberOfRecords;
            }

            var totalNumberOfPlayers = playerResults.Sum(x => x.NumberOfRecords);

            return new GamingGroupStats
            {
                TotalPlayedGames = totalPlayedGames,
                TotalNumberOfGamesWithPlays = totalNumberOfGamesWithPlays,
                TotalGamesOwned = numberOfGamesOwned,
                TotalNumberOfPlayersWithPlays = totalNumberOfPlayersWithPlays,
                TotalNumberOfPlayers = totalNumberOfPlayers 
            };
        }

        public GamingGroupWithUsers GetGamingGroupWithUsers(int gamingGroupId, ApplicationUser currentUser)
        {
            throw new System.NotImplementedException();
        }
    }
}