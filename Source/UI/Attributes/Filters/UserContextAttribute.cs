﻿#region LICENSE
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

using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.Web.Routing;
using UI.Controllers;

namespace UI.Attributes.Filters
{
    public class UserContextAttribute : ActionFilterAttribute
    {
        internal const string UserContextKey = "currentUser";

        private ApplicationUserManager _userManager;
        private ClientIdCalculator _clientIdCalculator;

        /// <summary>
        /// Indicates whether the action will send a redirect to the login page if the user isn't authenticated
        /// </summary>
        public bool RequiresGamingGroup { get; set; }

        public UserContextAttribute()
        {
            RequiresGamingGroup = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //TODO this doesn't appear to honor the HttpContextScoped structureMap directive... so it is getting a new
            // instance each time
            _userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();
            //TODO if there is only one implementation is this OK?
            _clientIdCalculator = new ClientIdCalculator();

            OnActionExecuting(filterContext, _userManager, _clientIdCalculator);

            base.OnActionExecuting(filterContext);
        }

        internal void OnActionExecuting(
            ActionExecutingContext filterContext, 
            ApplicationUserManager userManager,
            ClientIdCalculator clientIdCalculator)
        {
            if (filterContext.ActionParameters.ContainsKey(UserContextKey))
            {
                ApplicationUser applicationUser;

                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    var userId = filterContext.HttpContext.User.Identity.GetUserId();
                    applicationUser = userManager.FindByIdAsync(userId).Result;
                    if (RequiresGamingGroup 
                        && !applicationUser.CurrentGamingGroupId.HasValue
                        && !filterContext.IsChildAction)
                    {
                        var url = CreateManageAccountUrl(filterContext.RequestContext);

                        filterContext.Result = new RedirectResult(url);
                    }
                }
                else
                {
                    applicationUser = new AnonymousApplicationUser();
                    if (RequiresGamingGroup)
                    {
                        filterContext.Result = new RedirectToRouteResult(MVC.Account.Login().GetRouteValueDictionary());
                    }
                }

                applicationUser.AnonymousClientId = clientIdCalculator.GetClientId(filterContext.HttpContext.Request, applicationUser);

                filterContext.ActionParameters[UserContextKey] = applicationUser;
            }

            base.OnActionExecuting(filterContext);
        }

        internal virtual string CreateManageAccountUrl(RequestContext requestContext)
        {
            var url = UrlHelper.GenerateUrl(
                routeName: null,
                actionName: MVC.Account.ActionNames.Manage,
                controllerName: MVC.Account.Name,
                protocol: null,
                hostName: null,
                fragment: AccountController.GAMING_GROUPS_TAB_HASH_SUFFIX,
                routeValues: new RouteValueDictionary(new {message = AccountController.ManageMessageId.NoGamingGroup}),
                routeCollection: RouteTable.Routes,
                requestContext: requestContext,
                includeImplicitMvcValues: false);
            return url;
        }
    }
}