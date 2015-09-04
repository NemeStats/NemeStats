using System.Linq;

namespace BusinessLogic.Logic.Users
{
    public interface IAuthTokenGenerator
    {
        string GenerateAuthToken(string applicationUserId);
        string HashAuthToken(string authToken);
    }
}
