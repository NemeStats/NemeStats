using System.Collections.Generic;
using BusinessLogic.Models;
using System.Linq;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IGamingGroupRetriever
    {
        GamingGroupSummary GetGamingGroupDetails(int gamingGroupId, int maxNumberOfGamesToRetrieve);

        List<GamingGroup> GetGamingGroupsForUser(ApplicationUser applicationUser);
    }
}
