using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Exceptions
{
    public class ForeignKeyNotFoundException : Exception
    {
        public ForeignKeyNotFoundException(string expectedMessage) : base(expectedMessage)
        {
           
        }

        public ForeignKeyNotFoundException(string expectedMessage, Exception innerException)
            : base(expectedMessage, innerException)
        {

        }
    }
}
