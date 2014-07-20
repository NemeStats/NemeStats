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

namespace UI.Filters
{
    public class UserContextAttribute : ActionFilterAttribute
    {
        internal const string USER_CONTEXT_KEY = "currentUser";
        internal const string EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED = "User is not authenticated.";

        internal UserManager<ApplicationUser> userManager;

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
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED);
            }

            if (filterContext.ActionParameters.ContainsKey(USER_CONTEXT_KEY))
            {
                string userId = filterContext.HttpContext.User.Identity.GetUserId();
                ApplicationUser applicationUser = userManager.FindByIdAsync(userId).Result;

                if(RequiresGamingGroup)
                {
                    if (!applicationUser.CurrentGamingGroupId.HasValue)
                    {
                        filterContext.Result = new RedirectToRouteResult(MVC.GamingGroup.Create().GetRouteValueDictionary());
                    }
                }

                filterContext.ActionParameters[USER_CONTEXT_KEY] = applicationUser;
            }

            base.OnActionExecuting(filterContext);
        } 
    }
}