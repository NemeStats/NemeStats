using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Filters
{
    public class UserNameActionFilter : ActionFilterAttribute
    {
        internal const string UserNameKey = "userName";
        internal const string EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED = "User is not authenticated.";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED);
            }

            if (filterContext.ActionParameters.ContainsKey(UserNameKey))
            {
                filterContext.ActionParameters[UserNameKey] = filterContext.HttpContext.User.Identity.Name;
            }

            base.OnActionExecuting(filterContext);
        }

        
    }
}