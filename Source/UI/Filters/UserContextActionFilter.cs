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
    //TODO turn this into UserIdActionFilter. no need to use username since you can get the ID off identity.
    public class UserContextActionFilter : ActionFilterAttribute
    {
        internal const string USER_CONTEXT_KEY = "userContext";
        internal const string EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED = "User is not authenticated.";

        internal UserContextBuilder userContextBuilder = new UserContextBuilderImpl();

        //TODO get this dependency injection working through StructureMap instead of this hackiness.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                OnActionExecuting(filterContext, dbContext);
            }

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

                filterContext.ActionParameters[USER_CONTEXT_KEY] = userContext;
            }

            base.OnActionExecuting(filterContext);
        } 
    }
}