using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public interface IGamingGroupViewModelBuilder
    {
        GamingGroupViewModel Build(GamingGroupSummary gamingGroupSummary, ApplicationUser currentUser = null);
    }
}
