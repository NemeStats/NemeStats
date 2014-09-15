using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public interface IGamingGroupInviteConsumer
    {
        Task<int?> ConsumeGamingGroupInvitation(ApplicationUser currentUser);
    }
}
