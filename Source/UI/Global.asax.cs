#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System.Reflection;
using System.Web.Helpers;
using System.Web.Http;
using BusinessLogic.DataAccess;
using StructureMap.Web.Pipeline;
using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UI.App_Start;
using UI.Transformations;

namespace UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Configure anti-forgery cookie for modern SameSite behavior.
            // The project currently references Microsoft.AspNet.WebPages 3.2.3 (Feb 2015),
            // whose AntiForgeryConfig does not yet expose CookieSameSite. We use reflection
            // so the code compiles against the old package while still setting the property
            // at runtime on patched servers. Remove this reflection once packages are upgraded.
            var antiforgeryConfigType = typeof(AntiForgeryConfig);
            var cookieSameSiteProperty = antiforgeryConfigType.GetProperty("CookieSameSite", BindingFlags.Public | BindingFlags.Static);
            if (cookieSameSiteProperty != null)
            {
                cookieSameSiteProperty.SetValue(null, 0); // SameSiteMode.None
            }
            var requireSslProperty = antiforgeryConfigType.GetProperty("RequireSsl", BindingFlags.Public | BindingFlags.Static);
            if (requireSslProperty != null)
            {
                requireSslProperty.SetValue(null, true);
            }

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
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
