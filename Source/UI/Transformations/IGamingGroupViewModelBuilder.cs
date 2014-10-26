using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public interface IGamingGroupViewModelBuilder
    {
        GamingGroupViewModel Build(GamingGroup gamingGroup, ApplicationUser currentUser = null);
    }
}
