using BusinessLogic.Models.User;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public interface IGamingGroupInviteConsumer
    {
        Task<int?> ConsumeGamingGroupInvitation(ApplicationUser currentUser);
    }
}
