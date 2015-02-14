using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Security.OAuth;
using RIDGID.Blaze.PublicAPI.DependencyResolution;
using RollbarSharp;
using StructureMap;
using StructureMap.Graph;
using UI.DependencyResolution;

namespace UI.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            IContainer container = new Container(registry => registry.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.LookForRegistries();
            }));
            config.Services.Replace(typeof(IHttpControllerActivator), new StructureMapServiceActivator(config));
        }
    }
}