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

using BusinessLogic.Logic;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.GamingGroups;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models;
using UI.Models.User;

namespace UI.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        private ApplicationUserManager userManager;
        private readonly IUserRegisterer userRegisterer;
        private readonly IFirstTimeAuthenticator firstTimeAuthenticator;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IGamingGroupInviteConsumer gamingGroupInvitationConsumer;
        private readonly IGamingGroupRetriever _gamingGroupRetriever;

        public AccountController(
            ApplicationUserManager userManager,
            IUserRegisterer userRegisterer,
            IFirstTimeAuthenticator firstTimeAuthenticator,
            IAuthenticationManager authenticationManager,
            IGamingGroupInviteConsumer gamingGroupInvitationConsumer,
            IGamingGroupRetriever gamingGroupRetriever)
        {
            this.userManager = userManager;
            this.userRegisterer = userRegisterer;
            this.firstTimeAuthenticator = firstTimeAuthenticator;
            this.authenticationManager = authenticationManager;
            this.gamingGroupInvitationConsumer = gamingGroupInvitationConsumer;
            _gamingGroupRetriever = gamingGroupRetriever;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid? gamingGroupInvitation = null;
                if (!string.IsNullOrWhiteSpace(model.GamingGroupInvitationId))
                {
                    gamingGroupInvitation = new Guid(model.GamingGroupInvitationId);
                }

                NewUser newUser = new NewUser
                {
                    EmailAddress = model.EmailAddress.Trim(),
                    UserName = model.UserName.Trim(),
                    Password = model.Password,
                    GamingGroupInvitationId = gamingGroupInvitation
                };

                RegisterNewUserResult registerNewUserResult = await this.userRegisterer.RegisterUser(newUser);

                if (registerNewUserResult.Result.Succeeded)
                {
                    return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
                }
                this.AddErrors(registerNewUserResult.Result);
            }

            // If we got this far, something failed, redisplay form
            return View(MVC.Account.Views.Register, model);
        }

        [UserContext(RequiresGamingGroup = false)]
        [AllowAnonymous]
        public virtual ActionResult ConsumeInvitation(string id, ApplicationUser currentUser)
        {
            AddUserToGamingGroupResult result = gamingGroupInvitationConsumer.AddExistingUserToGamingGroup(id);

            if (result.UserAddedToExistingGamingGroup)
            {
                return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
            }

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                EmailAddress = result.EmailAddress,
                GamingGroupInvitationId = id
            };

            return this.View(MVC.Account.Views.RegisterAgainstExistingGamingGroup, registerViewModel);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await userManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public virtual ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your email has been changed."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error occurred."
                : "";

            SetViewBag();
            ManageAccountViewModel viewModel = GetBaseManageAccountViewModel();

            return View(MVC.Account.Views.Manage, viewModel);
        }

        public virtual ActionResult CreateGamingGroup()
        {
            return View(MVC.Account.Views.CreateGamingGroup);
        }

        [Authorize]
        [UserContext]
        public virtual ActionResult UserGamingGroups(ApplicationUser currentUser)
        {
            var gamingGroups = _gamingGroupRetriever.GetGamingGroupsForUser(currentUser);
            var model = new UserGamingGroupsModel
            {
                GamingGroups = gamingGroups,
                CurrentGamingGroup = gamingGroups.First(gg=>gg.Id == currentUser.CurrentGamingGroupId),
                CurrentUser = currentUser
            };
            return View(MVC.Account.Views.UserGamingGroups, model);
        }

        //TODO how to test async methods?
        // POST: /Account/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            var parentViewModel = GetBaseManageAccountViewModel();
            parentViewModel.PasswordViewModel = model;
            if (ModelState.IsValid)
            {
                var result = await userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result, "password");
            }

            // If we got this far, something failed, redisplay form
            SetViewBag();
            return View("Manage", parentViewModel);
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var parentViewModel = GetBaseManageAccountViewModel();
            parentViewModel.PasswordViewModel = model;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.ErrorEnterEmail });
            }
            var result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            SetViewBag();
            AddErrors(result, "password");
            return View("Manage", parentViewModel);
        }

        //TODO how to test async methods?
        // POST: /Account/ChangeEmailAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ChangeEmailAddress(ChangeEmailViewModel model)
        {
            var parentViewModel = GetBaseManageAccountViewModel();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await userManager.SetEmailAsync(User.Identity.GetUserId(), model.EmailAddress.Trim());
            if (result.Succeeded)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangeEmailSuccess });
            }
            AddErrors(result, "email");
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        private void SetViewBag()
        {
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
        }

        private ManageAccountViewModel GetBaseManageAccountViewModel()
        {
            ManageAccountViewModel viewModel = new ManageAccountViewModel();
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = userManager.FindById(currentUserId);
            viewModel.PasswordViewModel = HasPassword() ? (PasswordViewModel)new ChangePasswordViewModel() : new SetPasswordViewModel();
            ChangeEmailViewModel emailViewModel = new ChangeEmailViewModel();
            emailViewModel.EmailAddress = user.Email;
            viewModel.ChangeEmailViewModel = emailViewModel;

            return viewModel;
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await userManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);

                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await authenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await userManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await authenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser()
                {
                    UserName = model.UserName.Trim(),
                    Email = info.Email.Trim()
                };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(user, TransactionSource.WebApplication);

                        return RedirectToAction(MVC.GamingGroup.ActionNames.Index, "GamingGroup");
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public virtual ActionResult RemoveAccountList()
        {
            var linkedAccounts = userManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result, string validationKey = "")
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(validationKey, error);
            }
        }

        private bool HasPassword()
        {
            var user = userManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            SetPasswordSuccess,
            ChangePasswordSuccess,
            ChangeEmailSuccess,
            RemoveLoginSuccess,
            ErrorEnterEmail,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public virtual ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = passwordResetToken }, protocol: Request.Url.Scheme);
                await userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public virtual async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public virtual ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}