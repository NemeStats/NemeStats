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

namespace UI.App_Start
{
    public class GoogleAnalyticsConfig
    {
        public const string GOOGLE_TAG_MANAGER_TRACKING_ID_APP_KEY = "GoogleTagManager.TrackingId";

        private static string googleAnalyticsTrackingCode;

        private static readonly object gtmSyncRoot = new Object();
        private static string googleTagManagerTrackingCode;

        public static string GetGoogleTagManagerTrackingId()
        {
            if (googleTagManagerTrackingCode == null)
            {
                lock (gtmSyncRoot)
                {
                    if (googleTagManagerTrackingCode == null)
                    {
                        googleTagManagerTrackingCode = ConfigurationManager.AppSettings[GOOGLE_TAG_MANAGER_TRACKING_ID_APP_KEY];
                    }
                }
            }

            return googleTagManagerTrackingCode;
        }
    }
}