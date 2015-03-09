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

using System.Linq;
using System.Web.Mvc;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;

namespace UI.Attributes.Filters
{
    public class UserContextAttribute : ActionFilterAttribute
    {
        internal const string USER_CONTEXT_KEY = "currentUser";

        public ApplicationUserManager userManager;
        /// <summary>
        /// The parameter key for the Google Analytics unique client id.
        /// </summary>
        internal const string REQUEST_PARAM_ANALYTICS_ID = "_ga";

        public bool RequiresGamingGroup { get; set; }

        public UserContextAttribute()
        {
            this.RequiresGamingGroup = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //TODO this doesn't appear to honor the HttpContextScoped structureMap directive... so it is getting a new
            // instance each time
            this.userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();

            this.OnActionExecuting(filterContext, this.userManager);

            base.OnActionExecuting(filterContext);
        }

        internal void OnActionExecuting(
            ActionExecutingContext filterContext, 
            ApplicationUserManager userManager)
        {
            if (filterContext.ActionParameters.ContainsKey(USER_CONTEXT_KEY))
            {
                ApplicationUser applicationUser;

                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    string userId = filterContext.HttpContext.User.Identity.GetUserId();
                    applicationUser = userManager.FindByIdAsync(userId).Result;
                }else
                {
                    applicationUser = new AnonymousApplicationUser();
                }

                this.RedirectToLoginPageIfUserDoesntHaveAGamingGroup(filterContext, applicationUser);

                string anonymousClientId = filterContext.HttpContext.Request.Params[REQUEST_PARAM_ANALYTICS_ID];
                if(anonymousClientId == null)
                {
                    applicationUser.AnonymousClientId = UniversalAnalyticsNemeStatsEventTracker.DEFAULT_ANONYMOUS_CLIENT_ID;
                }
                else
                {
                    applicationUser.AnonymousClientId = anonymousClientId;
                }

                filterContext.ActionParameters[USER_CONTEXT_KEY] = applicationUser;
            }

            base.OnActionExecuting(filterContext);
        }

        private void RedirectToLoginPageIfUserDoesntHaveAGamingGroup(ActionExecutingContext filterContext, ApplicationUser applicationUser)
        {
            if (this.RequiresGamingGroup)
            {
                if (!applicationUser.CurrentGamingGroupId.HasValue)
                {
                    filterContext.Result = new RedirectToRouteResult(MVC.Account.Login().GetRouteValueDictionary());
                }
            }
        }
    }
}