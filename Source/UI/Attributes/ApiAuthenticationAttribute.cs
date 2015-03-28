using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Logic.Users;
using StructureMap;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace UI.Attributes
{
    public class ApiAuthenticationAttribute : ActionFilterAttribute
    {
        private readonly ApplicationUserManager applicationUserManager;
        internal const string AUTH_HEADER = "X-Auth-Token";

        public ApiAuthenticationAttribute()
            : this(DependencyResolver.Current.GetService<ApplicationUserManager>())
        {

        }

        public ApiAuthenticationAttribute(ApplicationUserManager applicationUserManager)
        {
            this.applicationUserManager = applicationUserManager;
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.Request.Headers.Contains(AUTH_HEADER))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest, 
                    "This action requires an " + AUTH_HEADER + " header.");

                return;
            }

            string authHeader = actionContext.Request.Headers.GetValues(AUTH_HEADER).FirstOrDefault();

            if(string.IsNullOrWhiteSpace(authHeader))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    "Invalid " + AUTH_HEADER); 
                return;
            }

            if (!this.applicationUserManager.Users.Any(x => x.AuthenticationToken == authHeader && DateTime.UtcNow <= x.AuthenticationTokenExpirationDate))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    "Invalid " + AUTH_HEADER);
            }
        }
    }
}