using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IUserRetriever
    {
        UserInformation RetrieveUserInformation(ApplicationUser applicationUser);
    }
}
