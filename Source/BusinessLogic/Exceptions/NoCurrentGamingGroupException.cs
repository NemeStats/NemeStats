using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Exceptions
{
    public class NoCurrentGamingGroupException : Exception
    {
        public NoCurrentGamingGroupException()
        {
        }

        public NoCurrentGamingGroupException(string message)
            : base(message)
        {
        }

        public NoCurrentGamingGroupException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
