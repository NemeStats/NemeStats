using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public interface IGamingGroupSaver
    {
        Task<GamingGroup> CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser);
        GamingGroup UpdateGamingGroupName(string gamingGroupName, ApplicationUser currentUser);
    }
}
