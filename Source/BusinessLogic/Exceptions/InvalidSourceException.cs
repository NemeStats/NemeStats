using System.Net;

namespace BusinessLogic.Exceptions
{
    public class InvalidSourceException : ApiFriendlyException
    {
        private const string ERROR_MESSAGE_FORMAT = "If either 'externalSourceApplicationName' or 'externalSourceEntityId' is set, then both must be set. "
            + "However, you passed externalSourceApplicationName:'{0}' and externalSourceEntityId:'{1}'.";

        public InvalidSourceException(string externalSourceApplicationName, string externalSourceEntityId) : 
            base(string.Format(ERROR_MESSAGE_FORMAT, externalSourceApplicationName, externalSourceEntityId), 
                HttpStatusCode.BadRequest)
        {
        }
    }
}
