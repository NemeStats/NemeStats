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
using System.Web;
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
            // Force Secure flag on the anti-forgery cookie.
            // Use reflection because AntiForgeryConfig.RequireSsl exists at runtime
            // but may not be in the reference assembly for WebPages 3.2.3.
            var antiforgeryConfigType = typeof(AntiForgeryConfig);
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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // If the client already has an anti-forgery cookie without SameSite,
            // the server won't re-send it. Explicitly overwrite it so the client
            // gets the updated SameSite=None + Secure attributes.
            var requestCookie = Request.Cookies["__RequestVerificationToken"];
            if (requestCookie != null)
            {
                var overwrite = new HttpCookie("__RequestVerificationToken", requestCookie.Value)
                {
                    HttpOnly = true,
                    Secure = true
                };
                var sameSiteProperty = typeof(HttpCookie).GetProperty("SameSite");
                if (sameSiteProperty != null)
                {
                    sameSiteProperty.SetValue(overwrite, 0); // SameSiteMode.None
                }
                Response.Cookies.Set(overwrite);
            }

            // Also intercept any new anti-forgery cookies the framework adds later
            // in this request (e.g. when rendering a fresh form).
            Response.AddOnSendingHeaders(context =>
            {
                var cookies = context.Response.Cookies;
                for (var i = 0; i < cookies.Count; i++)
                {
                    var cookie = cookies[i];
                    if (cookie.Name == "__RequestVerificationToken")
                    {
                        var sameSiteProp = typeof(HttpCookie).GetProperty("SameSite");
                        if (sameSiteProp != null)
                        {
                            sameSiteProp.SetValue(cookie, 0); // SameSiteMode.None
                        }
                        cookie.Secure = true;
                    }
                }
            });
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpContextLifecycle.DisposeAndClearAll();
        }
    }
}
