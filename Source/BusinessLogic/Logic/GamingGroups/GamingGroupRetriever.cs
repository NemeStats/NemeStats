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
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupRetriever : IGamingGroupRetriever
    {
        private readonly IDataContext dataContext;
        private readonly IPlayerRetriever playerRetriever;
        private readonly IGameDefinitionRetriever gameDefinitionRetriever;
        private readonly IPlayedGameRetriever playedGameRetriever;

        public GamingGroupRetriever(
            IDataContext dataContext,
            IPlayerRetriever playerRetriever,
            IGameDefinitionRetriever gameDefinitionRetriever,
            IPlayedGameRetriever playedGameRetriever)
        {
            this.dataContext = dataContext;
            this.playerRetriever = playerRetriever;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.playedGameRetriever = playedGameRetriever;
        }

        public GamingGroup GetGamingGroupById(int gamingGroupID)
        {
            var gamingGroup = dataContext.FindById<GamingGroup>(gamingGroupID);

            return gamingGroup;
        }

        public GamingGroupSummary GetGamingGroupDetails(GamingGroupFilter filter)
        {
            var gamingGroup = dataContext.FindById<GamingGroup>(filter.GamingGroupId);
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
            return dataContext.GetQueryable<GamingGroup>()
                              .Where(gamingGroup => gamingGroup.UserGamingGroups.Any(ugg => ugg.ApplicationUserId == applicationUser.Id))
                              .Select(gg => new GamingGroupListItemModel { Id = gg.Id, Name = gg.Name })
                              .ToList();
        }

        public List<TopGamingGroupSummary> GetTopGamingGroups(int numberOfTopGamingGroupsToShow)
        {
            return (from gamingGroup in dataContext.GetQueryable<GamingGroup>()
                    select new TopGamingGroupSummary
                    {
                        GamingGroupId = gamingGroup.Id,
                        GamingGroupName = gamingGroup.Name,
                        NumberOfGamesPlayed = gamingGroup.PlayedGames.Count,
                        NumberOfPlayers = gamingGroup.Players.Count,
                        GamingGroupChampion = gamingGroup.Players.OrderByDescending(p => p.PlayerGameResults.Select(pgr => pgr.TotalPoints).DefaultIfEmpty(0).Sum()).FirstOrDefault()
                    }).OrderByDescending(gg => gg.NumberOfGamesPlayed)
                      .ThenByDescending(gg => gg.NumberOfPlayers)
                      .Take(numberOfTopGamingGroupsToShow)
                      .ToList();
        }

        public List<GamingGroupSitemapInfo> GetGamingGroupsSitemapInfo()
        {
            return dataContext.GetQueryable<GamingGroup>()
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
    }
}