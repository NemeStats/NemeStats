using System.Linq;
using System.Web.Mvc;
using RollbarSharp;

namespace UI.Filters
{
    public class RollbarExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            (new RollbarClient()).SendException(filterContext.Exception);
        }
    }
}