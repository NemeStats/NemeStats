using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public interface IUserRegisterer
    {
        Task<IdentityResult> RegisterUser(NewUser newUser);
    }
}
