using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BusinessLogic.Logic;
using BusinessLogic.DataAccess;

namespace UI.Filters
{
    //TODO turn this into UserIdActionFilter. no need to use username since you can get the ID off identity.
    public class UserContextActionFilter : ActionFilterAttribute
    {
        internal const string USER_CONTEXT_KEY = "userContext";
        internal const string EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED = "User is not authenticated.";

        //TODO So very hacky, need to figure out how to implement something like this: http://lostechies.com/jimmybogard/2010/05/03/dependency-injection-in-asp-net-mvc-filters/
        internal UserContextBuilder userContextBuilder = new UserContextBuilderImpl();
        internal NemeStatsDbContext dbContext;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED);
            }

            if (filterContext.ActionParameters.ContainsKey(USER_CONTEXT_KEY))
            {
                bool injectedDbContext = true;
                if(dbContext == null)
                {
                    injectedDbContext = false;
                    dbContext = new NemeStatsDbContext();
                }

                UserContext userContext = userContextBuilder.GetUserContext(filterContext.HttpContext.User.Identity.GetUserId(), dbContext);

                if(!injectedDbContext)
                {
                    dbContext.Dispose();
                }

                filterContext.ActionParameters[USER_CONTEXT_KEY] = userContext;
            }

            base.OnActionExecuting(filterContext);
        }

        
    }
}