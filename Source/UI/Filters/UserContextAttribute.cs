using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BusinessLogic.Logic;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.EventTracking;

namespace UI.Filters
{
    public class UserContextAttribute : ActionFilterAttribute
    {
        internal const string USER_CONTEXT_KEY = "currentUser";
        internal const string EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED = "User is not authenticated.";

        internal UserManager<ApplicationUser> userManager;
        /// <summary>
        /// The parameter key for the Google Analytics unique client id.
        /// </summary>
        internal const string REQUEST_PARAM_ANALYTICS_ID = "_ga";

        public bool RequiresGamingGroup { get; set; }

        public UserContextAttribute()
        {
            RequiresGamingGroup = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            NemeStatsDbContext dbContext = DependencyResolver.Current.GetService<NemeStatsDbContext>();
            IUserStore<ApplicationUser> userStore = DependencyResolver.Current.GetService<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStore);

            OnActionExecuting(filterContext, userManager);

            base.OnActionExecuting(filterContext);
        }

        internal void OnActionExecuting(
            ActionExecutingContext filterContext, 
            UserManager<ApplicationUser> userManager)
        {
            ValidateUserIsAuthenticated(filterContext);

            if (filterContext.ActionParameters.ContainsKey(USER_CONTEXT_KEY))
            {
                string userId = filterContext.HttpContext.User.Identity.GetUserId();
                ApplicationUser applicationUser = userManager.FindByIdAsync(userId).Result;

                RedirectToGamingGroupCreatePageIfUserDoesntHaveAGamingGroup(filterContext, applicationUser);

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

        private static void ValidateUserIsAuthenticated(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED);
            }
        }

        private void RedirectToGamingGroupCreatePageIfUserDoesntHaveAGamingGroup(ActionExecutingContext filterContext, ApplicationUser applicationUser)
        {
            if (RequiresGamingGroup)
            {
                if (!applicationUser.CurrentGamingGroupId.HasValue)
                {
                    filterContext.Result = new RedirectToRouteResult(MVC.GamingGroup.Create().GetRouteValueDictionary());
                }
            }
        }
    }
}