using System.Net;

namespace BusinessLogic.Exceptions
{
    public class PlayerWithThisEmailAlreadyExistsException : ApiFriendlyException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "A User with email address '{0}' is already associated with the Player '{1}'. The existing playerId is '{2}'.";
        public PlayerWithThisEmailAlreadyExistsException(string emailAddress, string playerName, int playerId) 
            : base(string.Format(EXCEPTION_MESSAGE_FORMAT, emailAddress, playerName, playerId), HttpStatusCode.Conflict)
        {
        }
    }
}
