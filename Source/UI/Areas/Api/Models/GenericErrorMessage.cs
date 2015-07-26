using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap.Diagnostics;

namespace UI.Areas.Api.Models
{
    public class GenericErrorMessage
    {
        public GenericErrorMessage(string errorMessage)
        {
            Message = errorMessage;
        }

        public string Message { get; private set; }
    }
}