using BusinessLogic.DataAccess;
using Mindscape.Raygun4Net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //  don't want to set an initializer since we are doing code first with migrations 
            //  and the Configuration will call the DataSeeder.
            Database.SetInitializer<NerdScorekeeperDbContext>(null);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            new RaygunClient().Send(exception);
        }
    }
}
