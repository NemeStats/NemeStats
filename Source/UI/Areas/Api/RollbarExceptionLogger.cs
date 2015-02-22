using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.ExceptionHandling;
using RollbarSharp;

namespace UI.Areas.Api
{
    public class RollbarExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            (new RollbarClient()).SendException(context.Exception);
        }
    }
}
