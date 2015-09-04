using System;
using System.Linq;
using System.Net;

namespace BusinessLogic.Exceptions
{
    public abstract class ApiFriendlyException : Exception
    {
        public HttpStatusCode StatusCode;
        protected ApiFriendlyException(string friendlyMessage, HttpStatusCode statusCode) : base(friendlyMessage)
        {
            StatusCode = statusCode;
        }
    }
}
