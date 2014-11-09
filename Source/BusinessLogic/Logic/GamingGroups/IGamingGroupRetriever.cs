using BusinessLogic.Models;
using System.Linq;
using BusinessLogic.Models.GamingGroups;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IGamingGroupRetriever
    {
        GamingGroupSummary GetGamingGroupDetails(int gamingGroupId, int maxNumberOfGamesToRetrieve);
    }
}
