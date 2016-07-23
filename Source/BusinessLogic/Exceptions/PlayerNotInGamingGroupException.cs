using System.Net;

namespace BusinessLogic.Exceptions
{
    public class PlayerNotInGamingGroupException : ApiFriendlyException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "Player with id '{0}' is not in the Gaming Group with id '{1}'"
                                                         + " - which is the Gaming Group to which the Played Game is being"
                                                        + " recorded. All Players must be in this Gaming Group.";
        public PlayerNotInGamingGroupException(int playerId, int gamingGroupId) 
            : base(string.Format(EXCEPTION_MESSAGE_FORMAT, playerId, gamingGroupId), HttpStatusCode.Conflict)
        {
        }
    }
}
