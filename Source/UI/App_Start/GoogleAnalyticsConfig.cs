using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
using System.Web;

namespace UI.App_Start
{
    public class GoogleAnalyticsConfig
    {
        public const string UNIVERSAL_ANALYTICS_TRACKING_ID_APP_KEY = "UniversalAnalytics.TrackingId";

        private static readonly object syncRoot = new Object();
        private static string googleAnalyticsTrackingCode;

        public static string GetGoogleAnalyticsTrackingId()
        {
            if (googleAnalyticsTrackingCode == null)
            {
                lock (syncRoot)
                {
                    if (googleAnalyticsTrackingCode == null)
                    {
                        ConfigurationManager configManager = new ConfigurationManager();
                        googleAnalyticsTrackingCode = configManager.AppSettings[UNIVERSAL_ANALYTICS_TRACKING_ID_APP_KEY];
                    }
                }
            }

            return googleAnalyticsTrackingCode;
        }
    }
}