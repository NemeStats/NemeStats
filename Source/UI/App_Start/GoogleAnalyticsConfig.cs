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