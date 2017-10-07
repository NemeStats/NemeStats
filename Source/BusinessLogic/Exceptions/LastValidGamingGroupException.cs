using System.Net;

namespace BusinessLogic.Exceptions
{
    public class LastValidGamingGroupException : ApiFriendlyException
    {
        internal const string EXCEPTION_MESSAGE = "Cannot de-activate the last active Gaming Group with id '{0}' for the given User.";

        public LastValidGamingGroupException(int gamingGroupId) : base(string.Format(EXCEPTION_MESSAGE, gamingGroupId), HttpStatusCode.BadRequest)
        {
        }
    }
}