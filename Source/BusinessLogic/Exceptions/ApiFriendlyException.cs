using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
