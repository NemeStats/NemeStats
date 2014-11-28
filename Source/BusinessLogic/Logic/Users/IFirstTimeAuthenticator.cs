using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IFirstTimeAuthenticator
    {
        Task<object> CreateGamingGroupAndSendEmailConfirmation(ApplicationUser applicationUser);
    }
}
