using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Exceptions
{
    public class PlayerAlreadyExistsException : Exception
    {
        internal const string EXCEPTION_MESSAGE = "A Player with this name already exists.";

        public int ExistingPlayerId { get; set; }

        public PlayerAlreadyExistsException(int existingPlayerId)
            : base(EXCEPTION_MESSAGE)
        {
            ExistingPlayerId = existingPlayerId;
            
        }
    }
}
