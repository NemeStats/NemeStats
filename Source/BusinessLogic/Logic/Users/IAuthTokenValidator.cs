using BusinessLogic.Models;

namespace BusinessLogic.Logic.Users
{
    public interface IAuthTokenValidator
    {
        UserDeviceAuthToken ValidateAuthToken(string authToken);
    }
}