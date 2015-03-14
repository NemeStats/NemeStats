using System;
using System.Configuration;
using System.Configuration.Abstractions;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http.Routing;
using ConfigurationManager = System.Configuration.Abstractions.ConfigurationManager;

namespace UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ApiRouteAttribute : Attribute, IDirectRouteFactory, IHttpRouteInfoProvider
    {
        public int[] AcceptedVersions { get; set; }
        public string Name { get; set; }
        public string Template { get; private set; }
        public int Order { get; set; }
        internal string RoutePrefix { get; set; }

        internal IConfigurationManager ConfigurationManager = new ConfigurationManager();

        internal const string APP_KEY_CURRENT_API_VERSION = "currentApiVersion";

        public ApiRouteAttribute(string template)
        {
            ValidateTemplateIsNotNullOrBlank(template);
            if (template.StartsWith("/"))
            {
                throw new ArgumentException("The route cannot start with a forward slash ('/') since it will be prefixed with the api version (e.g. api/v2/).");
            }

            Template = template; 
        }

        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            Template = this.BuildRoute();
            Contract.Assert(context != null);

            IDirectRouteBuilder builder = context.CreateBuilder(Template);
            Contract.Assert(builder != null);

            builder.Name = Name;
            builder.Order = Order;
            return builder.Build();
        }

        private static void ValidateTemplateIsNotNullOrBlank(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException("template");
            }
        }

        internal virtual string BuildRoute()
        {
            if (AcceptedVersions == null)
            {
                AcceptedVersions = GetSupportedVersions(ConfigurationManager);
            }
            string routePrefix = "api/v{version:int:regex(" + string.Join("|", AcceptedVersions) + ")}";
            return routePrefix + "/" + Template;
        }

        private static int[] GetSupportedVersions(IConfigurationManager configurationManager)
        {
            string currentApiVersion = configurationManager.AppSettings.Get(APP_KEY_CURRENT_API_VERSION);

            if (string.IsNullOrWhiteSpace(currentApiVersion))
            {
                throw new ConfigurationErrorsException("The config file must have an appSetting with key = \"currentApiVersion\". ");
            }

            int currentVersion = int.Parse(configurationManager.AppSettings.Get(APP_KEY_CURRENT_API_VERSION));

            var supportedVersions = new int[currentVersion];

            for (int i = 0; i < currentVersion; i++)
            {
                supportedVersions[i] = i + 1;
            }
            return supportedVersions;
        }
    }
}