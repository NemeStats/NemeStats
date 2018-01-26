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
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace UI
{
    
    public partial class Startup
    {
        private const string NEMESTATS_API_PATH = "api/";
        private const int NINETY_DAYS = 90;

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = new TimeSpan(NINETY_DAYS, 0, 0, 0),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = rd =>
                    {
                        if ((HttpStatusCode) rd.Response.StatusCode != HttpStatusCode.Unauthorized ||
                            !rd.Request.Uri.AbsolutePath.ToLower().Contains(NEMESTATS_API_PATH))
                        {
                            rd.Response.Redirect(rd.RedirectUri);
                        }
                    }
                }
            });

            // AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //LoginPath = new PathString("/Account/Login"),
            //Provider = new CookieAuthenticationProvider
            //{
            //    OnApplyRedirect = ctx =>
            //    {
            //        if (!IsAjaxRequest(ctx.Request))
            //        {
            //            ctx.Response.Redirect(ctx.RedirectUri);
            //        }
            //    }
            //}

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            var googleAppId = ConfigurationManager.AppSettings.Get("googleAppId");
            var googleClientSecret = ConfigurationManager.AppSettings.Get("googleClientSecret");
            app.UseGoogleAuthentication(googleAppId, googleClientSecret);
        }
    }
}