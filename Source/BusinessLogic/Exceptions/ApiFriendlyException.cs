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

        /// <summary>
        /// Used to further distinguish between multiple possible instances of the same HTTP Status code
        /// </summary>
        public int? ErrorSubCode { get; set; }
    }
}
