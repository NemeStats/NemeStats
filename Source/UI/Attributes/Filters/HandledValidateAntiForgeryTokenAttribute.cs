#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace UI.Attributes.Filters
{
    public class HandledValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        internal const string AntiForgeryErrorKey = "AntiForgeryError";
        internal const string AntiForgeryErrorKindKey = "AntiForgeryError_kind";
        internal const string AntiForgeryErrorMessage = "This page expired. Please try again.";
        internal const string AntiForgeryErrorKind = "warning";

        private readonly AntiForgeryFailureMode _failureMode;

        public HandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode failureMode)
        {
            _failureMode = failureMode;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.Result != null)
            {
                return;
            }

            try
            {
                ValidateAntiForgeryToken();
            }
            catch (HttpAntiForgeryException)
            {
                filterContext.Result = GetFailureResult(filterContext);
            }
        }

        internal virtual void ValidateAntiForgeryToken()
        {
            AntiForgery.Validate();
        }

        private ActionResult GetFailureResult(AuthorizationContext filterContext)
        {
            switch (_failureMode)
            {
                case AntiForgeryFailureMode.RetryLogin:
                    SetRetryTempData(filterContext.Controller as Controller);
                    return new RedirectResult(GetRetryLoginUrl(filterContext));
                case AntiForgeryFailureMode.RetryRegister:
                    SetRetryTempData(filterContext.Controller as Controller);
                    return new RedirectResult(GetRetryRegisterUrl(filterContext));
                case AntiForgeryFailureMode.ForbiddenHtml:
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                case AntiForgeryFailureMode.ForbiddenJson:
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    return new JsonResult
                    {
                        Data = new
                        {
                            success = false,
                            message = AntiForgeryErrorMessage
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        private static void SetRetryTempData(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            controller.TempData[AntiForgeryErrorKey] = AntiForgeryErrorMessage;
            controller.TempData[AntiForgeryErrorKindKey] = AntiForgeryErrorKind;
        }

        private static string GetRetryLoginUrl(AuthorizationContext filterContext)
        {
            var origin = filterContext.HttpContext.Request.Form["origin"];

            switch (origin)
            {
                case "home":
                    return "/";
                case "login":
                    return "/Account/Login";
                case "register":
                    return "/Account/Register";
                default:
                    return "/Account/Login";
            }
        }

        private static string GetRetryRegisterUrl(AuthorizationContext filterContext)
        {
            var gamingGroupInvitationId = filterContext.HttpContext.Request.Form["GamingGroupInvitationId"];

            if (!string.IsNullOrWhiteSpace(gamingGroupInvitationId))
            {
                return $"/Account/ConsumeInvitation?id={gamingGroupInvitationId}";
            }

            return "/Account/Register";
        }
    }
}
