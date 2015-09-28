using System;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using UI.Areas.Api;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace UI.Attributes
{
    public class ApiAuthenticationAttribute : ActionFilterAttribute
    {
        private readonly IAuthTokenValidator authTokenValidator;
        private readonly ClientIdCalculator clientIdCalculator;
        
        internal const string AUTH_HEADER = "X-Auth-Token";
        internal const string PARAMETER_NAME_GAMING_GROUP_ID = "gamingGroupId";
        internal const string ERROR_MESSAGE_INVALID_AUTH_TOKEN = "Invalid " + AUTH_HEADER;
        internal const string ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP = "User does not have access to Gaming Group with Id '{0}'.";
        internal const string ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER = "This action requires an " + AUTH_HEADER + " header.";
        internal const string ERROR_MESSAGE_USER_MUST_HAVE_A_GAMING_GROUP = "The current user must be associated with a Gaming Group to call this action. Try authenticating and getting a new auth token."; 

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
            var apiBaseController = actionContext.ControllerContext.Controller as ApiControllerBase;
            if (apiBaseController == null)
            {
                throw new InvalidOperationException("The ApiAuthentication attribute can only be applied to actions in an ApiController that extends ApiControllerBase.");
            }

            if (!actionContext.Request.Headers.Contains(AUTH_HEADER))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER);

                return;
            }

            string authHeader = actionContext.Request.Headers.GetValues(AUTH_HEADER).FirstOrDefault();

            if(string.IsNullOrWhiteSpace(authHeader))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    ERROR_MESSAGE_INVALID_AUTH_TOKEN); 
                return;
            }

            ApplicationUser applicationUser = authTokenValidator.ValidateAuthToken(authHeader);

            if (applicationUser == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    ERROR_MESSAGE_INVALID_AUTH_TOKEN);
                return;
            }

            if (!applicationUser.CurrentGamingGroupId.HasValue)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                   HttpStatusCode.Unauthorized,
                   ApiAuthenticationAttribute.ERROR_MESSAGE_USER_MUST_HAVE_A_GAMING_GROUP);
                return; 
            }

            if (actionContext.ActionArguments.ContainsKey(PARAMETER_NAME_GAMING_GROUP_ID)
                && (int)actionContext.ActionArguments[PARAMETER_NAME_GAMING_GROUP_ID] != applicationUser.CurrentGamingGroupId)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    string.Format(ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP,
                    actionContext.ActionArguments[PARAMETER_NAME_GAMING_GROUP_ID]));
                return;
            }

            applicationUser.AnonymousClientId = this.clientIdCalculator.GetClientId(actionContext.Request, applicationUser);

            apiBaseController.CurrentUser = applicationUser;
        }
    }
}