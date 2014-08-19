using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface GamingGroupRetriever
    {
        GamingGroup GetGamingGroupDetails(int gamingGroupId, ApplicationUser currentUser);
    }
}
