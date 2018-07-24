using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using UI.Areas.Api;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace UI.Attributes
{
    public class ApiAuthenticationAttribute : ActionFilterAttribute
    {
        public bool AuthenticateOnly { get; set; }

        private readonly IAuthTokenValidator _authTokenValidator;
        private readonly ClientIdCalculator _clientIdCalculator;
        
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
            _authTokenValidator = authTokenValidator;
            _clientIdCalculator = clientIdCalculator;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var apiBaseController = actionContext.ControllerContext.Controller as ApiControllerBase;
            if (apiBaseController == null)
            {
                throw new InvalidOperationException("The ApiAuthentication attribute can only be applied to actions in an ApiController that extends ApiControllerBase.");
            }

            if (!actionContext.Request.Headers.Contains(AUTH_HEADER))
            {
                if (!AuthenticateOnly)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER);
                }

                return;
            }

            string authHeader = actionContext.Request.Headers.GetValues(AUTH_HEADER).FirstOrDefault();

            if(string.IsNullOrWhiteSpace(authHeader))
            {
                if (!AuthenticateOnly)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                        ERROR_MESSAGE_INVALID_AUTH_TOKEN);
                }

                return;
            }

            ApplicationUser applicationUser = _authTokenValidator.ValidateAuthToken(authHeader);

            if (applicationUser == null)
            {
                if (!AuthenticateOnly)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        ERROR_MESSAGE_INVALID_AUTH_TOKEN);
                }

                return;
            }

            if (actionContext.ActionArguments.ContainsKey(PARAMETER_NAME_GAMING_GROUP_ID)
                && (int)actionContext.ActionArguments[PARAMETER_NAME_GAMING_GROUP_ID] != applicationUser.CurrentGamingGroupId)
            {
                //TODO write unit test. This is a hot production fix
                if (!AuthenticateOnly)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.Unauthorized,
                        string.Format(ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP,
                            actionContext.ActionArguments[PARAMETER_NAME_GAMING_GROUP_ID]));
                }

                return;
            }

            applicationUser.AnonymousClientId = _clientIdCalculator.GetClientId(actionContext.Request, applicationUser);

            apiBaseController.CurrentUser = applicationUser;
        }
    }
}