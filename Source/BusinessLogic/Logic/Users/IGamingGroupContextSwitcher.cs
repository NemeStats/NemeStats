using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IGamingGroupContextSwitcher
    {
        void SwitchGamingGroupContext(int gamingGroupId, ApplicationUser currentUser);
    }
}
