using BusinessLogic.Models.User;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public interface IGamingGroupInviteConsumer
    {
        AddUserToGamingGroupResult AddExistingUserToGamingGroup(string gamingGroupInvitationId);

        void AddNewUserToGamingGroup(string applicationUserId, System.Guid guid);
    }
}
