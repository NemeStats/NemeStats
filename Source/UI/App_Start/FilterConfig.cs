using System.Web;
using System.Web.Mvc;
using UI.Filters;

namespace UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RollbarExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
