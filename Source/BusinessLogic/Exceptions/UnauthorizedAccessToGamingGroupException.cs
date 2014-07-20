using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Exceptions
{
    public class UnauthorizedAccessToGamingGroupException : Exception
    {
        internal const string EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAMING_GROUP
          = "User with Id '{0} is unauthorized to access '{1}' with Id '{2}'";

        public UnauthorizedAccessToGamingGroupException(string applicationUserId, string entityName, string entityId)
            
        {

        }

        public UnauthorizedAccessToGamingGroupException()
        {
        }

        public UnauthorizedAccessToGamingGroupException(string message)
            : base(message)
        {
        }
        public UnauthorizedAccessToGamingGroupException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
