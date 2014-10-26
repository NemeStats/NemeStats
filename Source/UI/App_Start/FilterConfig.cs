using System.Web.Mvc;
using UI.Filters;

namespace UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RollbarExceptionFilter(), 1);
            filters.Add(new HandleErrorAttribute(), 2);
        }
    }
}
