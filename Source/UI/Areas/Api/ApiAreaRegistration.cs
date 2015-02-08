using System.Web.Http;
using System.Web.Mvc;

namespace UI.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Api_default",
                "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                //new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}