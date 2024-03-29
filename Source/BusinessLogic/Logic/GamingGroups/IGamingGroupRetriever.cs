﻿#region LICENSE

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

#endregion LICENSE

using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using BusinessLogic.Models.Utility;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IGamingGroupRetriever
    {
        GamingGroupSummary GetGamingGroupDetails(GamingGroupFilter gamingGroupFilter);

        GamingGroup GetGamingGroupById(int gamingGroupId);

        IList<GamingGroupListItemModel> GetGamingGroupsForUser(string applicationUserId);

        List<TopGamingGroupSummary> GetTopGamingGroups(int numberOfTopGamingGroupsToShow);
        List<GamingGroupSitemapInfo> GetGamingGroupsSitemapInfo();
        GamingGroupStats GetGamingGroupStats(int gamingGroupId, BasicDateRangeFilter dateFilter);
        GamingGroupWithUsers GetGamingGroupWithUsers(int gamingGroupId, ApplicationUser currentUser);
        RecentGamingGroupChanges GetRecentChanges(int gamingGroupId, BasicDateRangeFilter dateFilter);
    }
}