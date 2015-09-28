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
using System;
using System.Configuration;
using System.Configuration.Abstractions;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http.Routing;
using ConfigurationManager = System.Configuration.Abstractions.ConfigurationManager;

namespace UI.Attributes
{
    /// <summary>
    /// This is a System.Attribute meant to be applied to REST actions as a means of specifying the Uri fragment
    /// of the resource (e.g. "SomeResource/") as well as a prefix for the version based on either the AcceptedVersions
    /// or StartingVersion being set. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ApiRouteAttribute : Attribute, IDirectRouteFactory, IHttpRouteInfoProvider
    {
        /// <summary>
        /// Designates the specific versions of the REST api to which this action should respond. For example, if
        /// the Acceptedversions are {1,2} then the the action will handle requests to /v1/{Resource} and 
        /// /v2/{Resource}. Cannot be used in conjunction with the StartingVersion property.
        /// </summary>
        public int[] AcceptedVersions { get; set; }
        /// <summary>
        /// Designates the first version of the API that supports this action. For example, if StartingVersion
        /// is set to 2 and the currentApiVersion in the config is set to 3, then this means the action will respond
        /// to requests to /v2/{Resource} and /v3/{Resource}. Cannot be used in conjunction with the AcceptedVersions
        /// property.
        /// </summary>
        public int StartingVersion { get; set; }
        /// <summary>
        /// The name of the route. Can be used to more conveniently generate links to the route. For more details see: http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-names
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A string pattern for the route template. This is constructed during route creation and doesn't need to be set manually. 
        /// </summary>
        public string Template { get; private set; }
        /// <summary>
        /// Represents a priority order for this route. The lower the number, the higher the priority. 
        /// In other words, when a request matches multiple routes it maps to the route with the lowest Order.
        /// For more details see: http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#order
        /// </summary>
        public int Order { get; set; }
        internal string RoutePrefix { get; set; }

        internal IConfigurationManager ConfigurationManager = new ConfigurationManager();

        internal const string APP_KEY_CURRENT_API_VERSION = "currentApiVersion";

        /// <summary>
        /// This constructor is automatically called when using the RouteAttribute annotation.
        /// </summary>
        /// <param name="template">The user-specified Uri template to which this action responds. Cannot start
        /// with a forward slash ('/')</param>
        /// <exception cref="ArgumentException">Throws an ArgumentException if the template starts with a forward-slash,
        /// since this is automatically added.</exception>
        public ApiRouteAttribute(string template)
        {
            ValidateTemplateIsNotNullOrBlank(template);
            if (template.StartsWith("/"))
            {
                throw new ArgumentException("The route cannot start with a forward slash ('/') since it will be prefixed with the api version (e.g. api/v2/).");
            }

            Template = template;
        }

        /// <summary>
        /// Constructs a RouteEntry from teh given context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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
                AcceptedVersions = GetSupportedVersions(StartingVersion, ConfigurationManager);
            }
            else
            {
                if (StartingVersion != 0)
                {
                    throw new InvalidOperationException("Either 'AcceptedVersions' or 'StartingVersion' can be set, but not both.");
                }
                if (AcceptedVersions.Any(version => version < 1))
                {
                    throw new InvalidOperationException("The explicitly specified AcceptedVersion values must all be positive integers.");
                }
            }
            string routePrefix = "api/v{version:int:regex(" + string.Join("|", AcceptedVersions) + ")}";
            return routePrefix + "/" + Template;
        }

        private static int[] GetSupportedVersions(int startVersion, IConfigurationManager configurationManager)
        {
            string currentApiVersionStringValue = configurationManager.AppSettings.AppSetting(APP_KEY_CURRENT_API_VERSION);

            int currentApiVersion = ValidateCurrentApiVersionAndStartVersion(startVersion, currentApiVersionStringValue);

            if (startVersion == 0)
            {
                startVersion = 1;
            }

            int numberOfVersions = 1 + currentApiVersion - startVersion;

            var supportedVersions = new int[numberOfVersions];

            for (int i = 0; i < numberOfVersions; i++)
            {
                supportedVersions[i] = startVersion + i;
            }
            return supportedVersions;
        }

        private static int ValidateCurrentApiVersionAndStartVersion(int? startVersion, string currentApiVersionStringValue)
        {
            int currentApiVersion;
            bool validInteger = int.TryParse(currentApiVersionStringValue, out currentApiVersion);

            if (!validInteger || currentApiVersion < 1)
            {
                throw new ConfigurationErrorsException("The 'currentApiVersion' app setting must be a positive integer.");
            }

            if (startVersion.HasValue && startVersion > currentApiVersion)
            {
                throw new InvalidOperationException("The 'StartingVersion' cannot be greater than the 'currentApiVersion' specified in the config.");
            }

            return currentApiVersion;
        }
    }
}