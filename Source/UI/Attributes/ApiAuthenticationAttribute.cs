using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using UI.Attributes.Filters;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace UI.Attributes
{
    public class ApiAuthenticationAttribute : ActionFilterAttribute
    {
        public const string ACTION_ARGUMENT_APPLICATION_USER = "applicationUser";

        private readonly IAuthTokenValidator authTokenValidator;
        private readonly ClientIdCalculator clientIdCalculator;
        
        internal const string AUTH_HEADER = "X-Auth-Token";
        internal const string UNAUTHORIZED_MESSAGE = "Invalid " + AUTH_HEADER;
        internal const string UNAUTHORIZED_TO_GAMING_GROUP_MESSAGE = "User does not have access to Gaming Group with Id '{0}'.";

        public ApiAuthenticationAttribute()
            : this(DependencyResolver.Current.GetService<AuthTokenValidator>(), new ClientIdCalculator())
        {

        }

        public ApiAuthenticationAttribute(IAuthTokenValidator authTokenValidator, ClientIdCalculator clientIdCalculator)
        {
            this.authTokenValidator = authTokenValidator;
            this.clientIdCalculator = clientIdCalculator;
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
                return;
            }

            if (actionContext.ActionArguments.ContainsKey("gamingGroupId")
                && (int)actionContext.ActionArguments["gamingGroupId"] != applicationUser.CurrentGamingGroupId)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    string.Format(UNAUTHORIZED_TO_GAMING_GROUP_MESSAGE, 
                    actionContext.ActionArguments["gamingGroupId"]));
                return;
            }

            applicationUser.AnonymousClientId = this.clientIdCalculator.GetClientId(actionContext.Request, applicationUser);

            actionContext.ActionArguments[ACTION_ARGUMENT_APPLICATION_USER] = applicationUser;
        }
    }
}