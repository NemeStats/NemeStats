using System.Collections.Generic;
using BusinessLogic.Models.GamingGroups;

namespace BusinessLogic.Facades
{
    public interface ITopGamingGroupsRetriever
    {
        List<TopGamingGroupSummary> GetResults(int numberOfTopGamingGroupsToShow);
    }
}
