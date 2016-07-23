using System;
using System.Net;

namespace BusinessLogic.Exceptions
{
    public abstract class ApiFriendlyException : ArgumentException
    {
        public HttpStatusCode StatusCode;
        protected ApiFriendlyException(string friendlyMessage, HttpStatusCode statusCode) : base(friendlyMessage)
        {
            StatusCode = statusCode;
        }
    }
}
