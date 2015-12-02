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

using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace UI.Attributes.Filters
{
    public class UserContextAttribute : ActionFilterAttribute
    {
        internal const string USER_CONTEXT_KEY = "currentUser";

        private ApplicationUserManager userManager;
        private ClientIdCalculator clientIdCalculator;

        public bool RequiresGamingGroup { get; set; }

        public UserContextAttribute()
        {
            this.RequiresGamingGroup = true;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //TODO this doesn't appear to honor the HttpContextScoped structureMap directive... so it is getting a new
            // instance each time
            this.userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();
            //TODO if there is only one implementation is this OK?
            this.clientIdCalculator = new ClientIdCalculator();

            this.OnActionExecuting(filterContext, this.userManager, clientIdCalculator);

            base.OnActionExecuting(filterContext);
        }

        internal void OnActionExecuting(
            ActionExecutingContext filterContext, 
            ApplicationUserManager userManager,
            ClientIdCalculator clientIdCalculator)
        {
            if (filterContext.ActionParameters.ContainsKey(USER_CONTEXT_KEY))
            {
                ApplicationUser applicationUser;

                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    string userId = filterContext.HttpContext.User.Identity.GetUserId();
                    applicationUser = userManager.FindByIdAsync(userId).Result;
                }else
                {
                    applicationUser = new AnonymousApplicationUser();
                    if (this.RequiresGamingGroup)
                    {
                        filterContext.Result = new RedirectToRouteResult(MVC.Account.Login().GetRouteValueDictionary());
                    }
                }

                applicationUser.AnonymousClientId = clientIdCalculator.GetClientId(filterContext.HttpContext.Request, applicationUser);

                filterContext.ActionParameters[USER_CONTEXT_KEY] = applicationUser;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}