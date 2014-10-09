using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public interface IFirstTimeAuthenticator
    {
        Task<object> SignInAndCreateGamingGroup(Models.User.ApplicationUser applicationUser);
    }
}
