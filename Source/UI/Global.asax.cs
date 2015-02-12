using System.Web.Http;
using BusinessLogic.DataAccess;
using StructureMap;
using StructureMap.Web.Pipeline;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UI.App_Start;
using UI.DependencyResolution;
using UI.Transformations;

namespace UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //  don't want to set an initializer since we are doing code first with migrations 
            //  and the Configuration will call the DataSeeder.
            Database.SetInitializer<NemeStatsDbContext>(null);
            AutomapperConfiguration.Configure();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpContextLifecycle.DisposeAndClearAll();
        }
    }
}
