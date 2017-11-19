using System.Net;

namespace BusinessLogic.Exceptions
{
    public class ApiAuthenticationException : ApiFriendlyException
    {
        public ApiAuthenticationException(string authTokenHeaderName) : base($"Invalid {authTokenHeaderName}", HttpStatusCode.Unauthorized)
        {
        }
    }
}
