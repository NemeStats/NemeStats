using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public interface IUserRegisterer
    {
        Task<IdentityResult> RegisterUser(NewUser newUser);
    }
}
