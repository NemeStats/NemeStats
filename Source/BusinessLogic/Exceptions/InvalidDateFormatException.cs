using System.Net;

namespace BusinessLogic.Exceptions
{
    public class InvalidDateFormatException : ApiFriendlyException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "Dates should be in the YYYY-MM-DD (ISO-8601) date format, but the input date was '{0}'.";

        public InvalidDateFormatException(string invalidDateInput) : base(string.Format(EXCEPTION_MESSAGE_FORMAT, invalidDateInput), HttpStatusCode.BadRequest)
        {
        }
    }
}
