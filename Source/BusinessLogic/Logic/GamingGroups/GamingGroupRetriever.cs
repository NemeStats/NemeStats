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

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupRetriever : IGamingGroupRetriever
    {
        private readonly IDataContext _dataContext;

        public GamingGroupRetriever(
            IDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public GamingGroup GetGamingGroupById(int gamingGroupID)
        {
            var gamingGroup = _dataContext.FindById<GamingGroup>(gamingGroupID);

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
                              .Where(gamingGroup => gamingGroup.UserGamingGroups.Any(ugg => ugg.ApplicationUserId == applicationUser.Id))
                              .Select(gg => new GamingGroupListItemModel { Id = gg.Id, Name = gg.Name })
                              .ToList();
        }

        public List<TopGamingGroupSummary> GetTopGamingGroups(int numberOfTopGamingGroupsToShow)
        {
            return (from gamingGroup in _dataContext.GetQueryable<GamingGroup>()
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

        public GamingGroupStats GetGamingGroupStats(int gamingGroupId)
        {
            var results = _dataContext.GetQueryable<PlayedGame>()
                .Where(x => x.GamingGroupId == gamingGroupId)
                .GroupBy(x => x.GameDefinitionId)
                .Select(g => new
                {
                    Id = g.Key,
                    NumberOfGamesPlayed = g.Count()
                }).ToList();

            if (results.Count == 0)
            {
                return GamingGroupStats.NullStats;
            }

            var result = new GamingGroupStats
            {
                TotalPlayedGames = results.Sum(x => x.NumberOfGamesPlayed),
                DistinctGamesPlayed = results.Distinct().Count()
            };


            //result.DistinctGamesPlayed = _dataContext.GetQueryable<GameDefinition>()
            //    .Where(x => x.GamingGroupId == gamingGroupId)
            //    .GroupBy(x => x.Id)
            //    .Select(x => x.Key).Count();

            return result;
        }
    }
}