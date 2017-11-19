using System.Net;

namespace BusinessLogic.Exceptions
{
    public class UserHasNoGamingGroupException : ApiFriendlyException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "User with id '{0}' does not have an Active Gaming Group!";
        public UserHasNoGamingGroupException(string applicationUserId) : base(string.Format(EXCEPTION_MESSAGE_FORMAT, applicationUserId), HttpStatusCode.BadRequest)
        {
        }
    }
}
