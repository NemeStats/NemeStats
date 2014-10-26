using BusinessLogic.Models;
using System.Linq;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IGamingGroupRetriever
    {
        GamingGroup GetGamingGroupDetails(int gamingGroupId, int maxNumberOfGamesToRetrieve);
    }
}
