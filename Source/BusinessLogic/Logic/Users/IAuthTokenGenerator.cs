using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IAuthTokenGenerator
    {
        AuthToken GenerateAuthToken(string applicationUserId, string uniqueDeviceId = null);
        string HashAuthToken(string authToken);
    }
}
