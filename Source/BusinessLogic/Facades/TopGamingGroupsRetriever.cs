using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.GamingGroups;

namespace BusinessLogic.Facades
{
    public class TopGamingGroupsRetriever : Cacheable<int, List<TopGamingGroupSummary>>, ITopGamingGroupsRetriever
    {
        private readonly IGamingGroupRetriever _gamingGroupRetriever;

        public TopGamingGroupsRetriever(
            IGamingGroupRetriever gamingGroupRetriever,
            IDateUtilities dateUtilities,
            ICacheService cacheService) : base(dateUtilities, cacheService)
        {
            _gamingGroupRetriever = gamingGroupRetriever;
        }

        public override List<TopGamingGroupSummary> GetFromSource(int numberOfGamingGroupsToRetrieve)
        {
            return _gamingGroupRetriever.GetTopGamingGroups(numberOfGamingGroupsToRetrieve);
        }
    }
}