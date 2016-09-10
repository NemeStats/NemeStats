using System.Net;

namespace BusinessLogic.Exceptions
{
    public class EntityAlreadySynchedException : ApiFriendlyException
    {
        private const string ERROR_MESSAGE_FORMAT =
            "An entity with an ApplicationName of '{0}' and an EntityId of '{1}' already exists in the Gaming Group with id '{2}'.";

        public EntityAlreadySynchedException(string applicationName, string entityId, int gamingGroupId)
            : base(string.Format(ERROR_MESSAGE_FORMAT, applicationName, entityId, gamingGroupId), 
                  HttpStatusCode.Conflict)
        {
        }
    }
}
