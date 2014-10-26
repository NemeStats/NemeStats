using System.Linq;
using System.Web.Mvc;
using RollbarSharp;

namespace UI.Filters
{
    public class RollbarExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            (new RollbarClient()).SendException(filterContext.Exception);
        }
    }
}