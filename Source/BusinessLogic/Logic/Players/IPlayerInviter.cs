using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Players
{
    public interface IPlayerInviter
    {
        void InvitePlayer(PlayerInvitation playerInvitation,ApplicationUser applicationUser);
    }
}
