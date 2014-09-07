using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public interface GamingGroupToGamingGroupViewModelTransformation
    {
        GamingGroupViewModel Build(GamingGroup gamingGroup, ApplicationUser currentUser = null);
    }
}
