using System.Net;

namespace BusinessLogic.Exceptions
{
    public class NoValidGamingGroupException : ApiFriendlyException
    {
        public NoValidGamingGroupException(string userId) 
            : base($"User with id '{userId}' could not create the new entity because no Gaming Group Id was specified.", HttpStatusCode.BadRequest)
        {
        }
    }
}
