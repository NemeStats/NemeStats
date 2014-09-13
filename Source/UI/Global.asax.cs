using Bugsnag.Library;
using BusinessLogic.DataAccess;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;
using RollbarSharp;
using StructureMap;
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
        internal const string INFO_MESSAGE_TO_LOG = "This is just an informational message. A request was sent to: {0}";
        internal const string INFORMATIONAL_TAG = "InfoTag";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //  don't want to set an initializer since we are doing code first with migrations 
            //  and the Configuration will call the DataSeeder.
            Database.SetInitializer<NemeStatsDbContext>(null);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError().GetBaseException();

            (new RollbarClient()).SendException(exception);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //TODO do something to displose of structuremap objects here
        }
    }
}
