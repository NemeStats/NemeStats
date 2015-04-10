using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IAuthTokenValidator
    {
        ApplicationUser ValidateAuthToken(string authToken);
    }
}