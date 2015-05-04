using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Exceptions
{
    public abstract class ApiFriendlyException : Exception
    {
        protected ApiFriendlyException(string friendlyMessage) : base(friendlyMessage)
        {
            
        }
    }
}
