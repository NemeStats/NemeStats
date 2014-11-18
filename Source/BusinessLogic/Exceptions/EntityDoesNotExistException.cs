using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Exceptions
{
    public class EntityDoesNotExistException : KeyNotFoundException
    {
        internal const string EXCEPTION_MESSAGE_FORMAT = "Entity with Id {0} does not exist.";

        private readonly object entityId;

        public EntityDoesNotExistException(object entityId)
        {
            this.entityId = entityId;
        }

        public override string Message
        {
            get
            {
                return string.Format(EXCEPTION_MESSAGE_FORMAT, entityId);
            }
        }
    }
}
