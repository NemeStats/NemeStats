using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IFirstTimeAuthenticator
    {
        Task<object> SignInAndCreateGamingGroup(ApplicationUser applicationUser);
    }
}
