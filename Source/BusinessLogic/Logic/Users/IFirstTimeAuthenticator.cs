using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IFirstTimeAuthenticator
    {
        Task<object> SignInAndCreateGamingGroup(ApplicationUser applicationUser);
    }
}
