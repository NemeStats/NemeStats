using System;
using System.Net;

namespace BusinessLogic.Exceptions
{
    public class InvalidPlayedGameDateException : ApiFriendlyException
    {
        public InvalidPlayedGameDateException(DateTime datePlayed) : base($"'{datePlayed.Date}' is not a valid date for a Played Game. A play cannot be recorded more than 1 day in advance.", HttpStatusCode.BadRequest)
        {
        }
    }
}