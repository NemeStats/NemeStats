using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace UI.Attributes
{
    public class ApiAuthenticationAttribute : ActionFilterAttribute
    {
        public const string ACTION_ARGUMENT_APPLICATION_USER = "applicationUser";

        private readonly IAuthTokenValidator authTokenValidator;
        internal const string AUTH_HEADER = "X-Auth-Token";
        internal const string UNAUTHORIZED_MESSAGE = "Invalid " + AUTH_HEADER;

        public ApiAuthenticationAttribute()
            : this(DependencyResolver.Current.GetService<AuthTokenValidator>())
        {

        }

        public ApiAuthenticationAttribute(IAuthTokenValidator authTokenValidator)
        {
            this.authTokenValidator = authTokenValidator;
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
                    UNAUTHORIZED_MESSAGE); 
                return;
            }

            ApplicationUser applicationUser = authTokenValidator.ValidateAuthToken(authHeader);

            if (applicationUser == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    UNAUTHORIZED_MESSAGE);
            }

            actionContext.ActionArguments[ACTION_ARGUMENT_APPLICATION_USER] = applicationUser;
        }
    }
}