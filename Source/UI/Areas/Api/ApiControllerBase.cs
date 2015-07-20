using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace UI.Areas.Api
{
    public class ApiControllerBase : ApiController
    {
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            //if (!actionContext.Request.Headers.Contains(AUTH_HEADER))
            //{
            //    actionContext.Response = actionContext.Request.CreateErrorResponse(
            //        HttpStatusCode.BadRequest,
            //        ERROR_MESSAGE_MISSING_AUTH_TOKEN_HEADER);

            //    return;
            //}

            //string authHeader = actionContext.Request.Headers.GetValues(AUTH_HEADER).FirstOrDefault();

            //if (string.IsNullOrWhiteSpace(authHeader))
            //{
            //    actionContext.Response = actionContext.Request.CreateErrorResponse(
            //        HttpStatusCode.BadRequest,
            //        ERROR_MESSAGE_INVALID_AUTH_TOKEN);
            //    return;
            //}

            //ApplicationUser applicationUser = authTokenValidator.ValidateAuthToken(authHeader);

            //if (applicationUser == null)
            //{
            //    actionContext.Response = actionContext.Request.CreateErrorResponse(
            //        HttpStatusCode.Unauthorized,
            //        ERROR_MESSAGE_INVALID_AUTH_TOKEN);
            //    return;
            //}

            //if (actionContext.ActionArguments.ContainsKey(PARAMETER_NAME_GAMING_GROUP_ID)
            //    && (int)actionContext.ActionArguments[PARAMETER_NAME_GAMING_GROUP_ID] != applicationUser.CurrentGamingGroupId)
            //{
            //    actionContext.Response = actionContext.Request.CreateErrorResponse(
            //        HttpStatusCode.Unauthorized,
            //        string.Format(ERROR_MESSAGE_UNAUTHORIZED_TO_GAMING_GROUP,
            //        actionContext.ActionArguments[PARAMETER_NAME_GAMING_GROUP_ID]));
            //    return;
            //}

            //applicationUser.AnonymousClientId = this.clientIdCalculator.GetClientId(actionContext.Request, applicationUser);

            //actionContext.ActionArguments[ACTION_ARGUMENT_APPLICATION_USER] = applicationUser;
        }
    }
}