using BusinessLogic.Models.User;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BusinessLogic.Logic.Users;
using BusinessLogic.EventTracking;

namespace UI.Filters
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
            RequiresGamingGroup = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //TODO this doesn't appear to honor the HttpContextScoped structureMap directive... so it is getting a new
            // instance each time
            userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();

            OnActionExecuting(filterContext, userManager);

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

                RedirectToLoginPageIfUserDoesntHaveAGamingGroup(filterContext, applicationUser);

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
            if (RequiresGamingGroup)
            {
                if (!applicationUser.CurrentGamingGroupId.HasValue)
                {
                    filterContext.Result = new RedirectToRouteResult(MVC.Account.Login().GetRouteValueDictionary());
                }
            }
        }
    }
}