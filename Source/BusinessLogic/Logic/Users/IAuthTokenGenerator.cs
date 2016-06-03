using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IAuthTokenGenerator
    {
        AuthToken GenerateAuthToken(string applicationUserId);
        string HashAuthToken(string authToken);
    }
}
