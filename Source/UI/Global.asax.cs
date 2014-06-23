using Bugsnag.Library;
using BusinessLogic.DataAccess;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;
using RollbarSharp;
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
            var exception = Server.GetLastError();
            LogErrorToRayGun(exception);
            LogErrorToBugSnag(exception);
            LogErrorToRollBar(exception);

        }

        private static void LogErrorToRayGun(Exception exception)
        {
            try
            {
                RaygunClient raygunClient = new RaygunClient();
                raygunClient.Send(exception);
            }
            catch (Exception) { }
        }

        private static void LogErrorToBugSnag(Exception exception)
        {
            try
            {
                BugSnag bugSnagClient = new BugSnag("839b07c129a3c860855dda0efe3505fe");
                bugSnagClient.Notify(exception, "From-Dev");
            }
            catch (Exception) { }
        }

        private static void LogErrorToRollBar(Exception exception)
        {
            try
            {
                RollbarClient rollbarClient = new RollbarClient("ea76150e8b7741ea9433b3052dc861a2");
                rollbarClient.SendErrorException(exception);
            }
            catch (Exception) { }
        }

        //TODO Remove this when error logger bake-off is over. Just using this to test logging informational messages
        protected void Application_BeginRequest()
        {
            if (DateTime.Now.Millisecond % 2 == 0)
            {
                string infoMessage = string.Format(INFO_MESSAGE_TO_LOG, Request.RawUrl);
                LogRayGunInformationalMessage(infoMessage);
                LogBugSnagInformationalMessage(infoMessage);
                LogRollbarInformationalMessage(infoMessage);
            }
        }

        private static void LogRayGunInformationalMessage(string infoMessageToLog)
        {
            try
            {
                RaygunClient raygunClient = new RaygunClient();
                raygunClient.SendInBackground(new Exception(infoMessageToLog), new List<string> { INFORMATIONAL_TAG });
            }
            catch (Exception) { }

        }

        private static void LogRollbarInformationalMessage(string infoMessageToLog)
        {
            try
            {
                RollbarClient rollbarClient = new RollbarClient("ea76150e8b7741ea9433b3052dc861a2");
                Dictionary<string, object> customDataDictionary = new Dictionary<string, object>();
                customDataDictionary.Add(INFORMATIONAL_TAG, "infoTag");
                rollbarClient.SendInfoMessage(infoMessageToLog, customDataDictionary);
            }
            catch (Exception) { }
        }

        private static void LogBugSnagInformationalMessage(string infoMessageToLog)
        {
            try
            {
                //TODO Unfortunately getting ArgumentNullException if trying to pass extra data.
                BugSnag bugSnagClient = new BugSnag("839b07c129a3c860855dda0efe3505fe");
                bugSnagClient.Notify(new Exception(infoMessageToLog));
            }
            catch (Exception) { }
        }


    }
}
