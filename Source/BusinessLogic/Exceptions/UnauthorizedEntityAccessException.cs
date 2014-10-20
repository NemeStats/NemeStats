using System;
using System.Linq;

namespace BusinessLogic.Exceptions
{
    public class UnauthorizedEntityAccessException : UnauthorizedAccessException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "User with Id '{0}' does not have access to entity of type '{1}' with Id '{2}";

        private string userId;
        private Type entityType;
        private object entityId;

        public UnauthorizedEntityAccessException(string userId, Type entityType, object entityId)
        {
            this.userId = userId;
            this.entityType = entityType;
            this.entityId = entityId;
        }

        public override string Message
        {
            get
            {
                return string.Format(EXCEPTION_MESSAGE_FORMAT, userId, entityType, entityId);
            }
        }
    }
}
