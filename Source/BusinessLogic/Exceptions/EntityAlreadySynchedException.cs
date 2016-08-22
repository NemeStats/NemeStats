using System.Net;
using BusinessLogic.Models;

namespace BusinessLogic.Exceptions
{
    public class EntityAlreadySynchedException : ApiFriendlyException
    {
        private const string ERROR_MESSAGE_FORMAT =
            "An entity with an ExternalSourceApplicationName of '{0}' and an ExternalSourceEntityId of '{1}' already exists in the Gaming Group with id '{2}'.";

        public EntityAlreadySynchedException(ISynchable entityBeingSynched, int gamingGroupId)
            : base(string.Format(ERROR_MESSAGE_FORMAT, entityBeingSynched.ExternalSourceApplicationName,
                    entityBeingSynched.ExternalSourceEntityId, gamingGroupId), 
                  HttpStatusCode.Conflict)
        {
        }
    }
}
