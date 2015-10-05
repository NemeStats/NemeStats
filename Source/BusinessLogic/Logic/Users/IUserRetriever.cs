using System.Linq;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IUserRetriever
    {
        UserInformation RetrieveUserInformation(string expectedUserID, ApplicationUser applicationUser);
    }
}
