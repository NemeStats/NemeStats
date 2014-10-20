using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
namespace BusinessLogic.Logic.GamingGroups
{
    public interface IPendingGamingGroupInvitationRetriever
    {
        List<GamingGroupInvitation> GetPendingGamingGroupInvitations(ApplicationUser currentUser);
    }
}
