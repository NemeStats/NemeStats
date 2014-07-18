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
        internal const string USER_CONTEXT_KEY = "userContext";
        internal const string EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED = "User is not authenticated.";

        internal UserContextBuilder userContextBuilder = new UserContextBuilderImpl();

        public bool RequiresGamingGroup { get; set; }

        public UserContextAttribute()
        {
            RequiresGamingGroup = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            NemeStatsDbContext dbContext = DependencyResolver.Current.GetService<NemeStatsDbContext>();

            OnActionExecuting(filterContext, dbContext);

            base.OnActionExecuting(filterContext);
        }

        internal void OnActionExecuting(ActionExecutingContext filterContext, NemeStatsDbContext dbContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED);
            }

            if (filterContext.ActionParameters.ContainsKey(USER_CONTEXT_KEY))
            {
                UserContext userContext = userContextBuilder.GetUserContext(filterContext.HttpContext.User.Identity.GetUserId(), dbContext);

                if(RequiresGamingGroup)
                {
                    if(!userContext.GamingGroupId.HasValue)
                    {
                        filterContext.Result = new RedirectToRouteResult(MVC.GamingGroup.Create().GetRouteValueDictionary());
                    }
                }

                filterContext.ActionParameters[USER_CONTEXT_KEY] = userContext;
            }

            base.OnActionExecuting(filterContext);
        } 
    }
}