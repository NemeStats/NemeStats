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
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GamingGroups;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models;
using UI.Models.GamingGroup;
using UI.Models.User;

namespace UI.Controllers
{
    [Authorize]
    public partial class AccountController : BaseController
    {
        public const string GAMING_GROUPS_TAB_HASH_SUFFIX = "gaming-groups";

        private readonly ApplicationUserManager _userManager;
        private readonly IUserRegisterer _userRegisterer;
        private readonly IFirstTimeAuthenticator _firstTimeAuthenticator;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IGamingGroupInviteConsumer _gamingGroupInvitationConsumer;
        private readonly IGamingGroupRetriever _gamingGroupRetriever;
        private readonly IBoardGameGeekUserSaver _boardGameGeekUserSaver;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;
        private readonly IUserRetriever _userRetriever;
        private readonly ITransformer _transformer;
        private readonly IGamingGroupContextSwitcher _gamingGroupContextSwitcher;

        public AccountController(
            ApplicationUserManager userManager,
            IUserRegisterer userRegisterer,
            IFirstTimeAuthenticator firstTimeAuthenticator,
            IAuthenticationManager authenticationManager,
            IGamingGroupInviteConsumer gamingGroupInvitationConsumer,
            IGamingGroupRetriever gamingGroupRetriever,
            IBoardGameGeekUserSaver boardGameGeekUserSaver,
            IBoardGameGeekApiClient boardGameGeekApiClient,
            IUserRetriever userRetriever, 
            ITransformer transformer, 
            IGamingGroupContextSwitcher gamingGroupContextSwitcher)
        {
            _userManager = userManager;
            _userRegisterer = userRegisterer;
            _firstTimeAuthenticator = firstTimeAuthenticator;
            _authenticationManager = authenticationManager;
            _gamingGroupInvitationConsumer = gamingGroupInvitationConsumer;
            _gamingGroupRetriever = gamingGroupRetriever;
            _boardGameGeekUserSaver = boardGameGeekUserSaver;
            _boardGameGeekApiClient = boardGameGeekApiClient;
            _userRetriever = userRetriever;
            _transformer = transformer;
            _gamingGroupContextSwitcher = gamingGroupContextSwitcher;
        }

        [AllowAnonymous]
        public virtual ActionResult LoginForm()
        {
            var model = new LoginViewModel();
            return View(MVC.Account.Views.LoginForm, model);
        }

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
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);

                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
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

                var newUser = new NewUser
                {
                    EmailAddress = model.EmailAddress.Trim(),
                    UserName = model.UserName.Trim(),
                    Password = model.Password,
                    GamingGroupInvitationId = gamingGroupInvitation
                };

                var registerNewUserResult = await _userRegisterer.RegisterUser(newUser);

                if (registerNewUserResult.Result.Succeeded)
                {
                    return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
                }
                AddErrors(registerNewUserResult.Result);
            }

            // If we got this far, something failed, redisplay form
            return View(MVC.Account.Views.Register, model);
        }

        [UserContext(RequiresGamingGroup = false)]
        [AllowAnonymous]
        public virtual ActionResult ConsumeInvitation(string id, ApplicationUser currentUser)
        {
            var result = _gamingGroupInvitationConsumer.AddExistingUserToGamingGroup(id);

            if (result.UserAddedToExistingGamingGroup)
            {
                return RedirectToAction(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name);
            }

            var registerViewModel = new RegisterViewModel
            {
                EmailAddress = result.EmailAddress,
                GamingGroupInvitationId = id
            };

            return View(MVC.Account.Views.RegisterAgainstExistingGamingGroup, registerViewModel);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            var result = await _userManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
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

        // GET: /Account/Manage
        public virtual ActionResult Manage(ManageMessageId? message)
        {
            var tempMessage =
                message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetBoardGameGeekUserSuccess ? "Your BGG account has been linked with NemeStats successfully. Now you can import your games on the Gaming Group page."
                : message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your email has been changed."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error occurred."
                : message == ManageMessageId.NoGamingGroup ? "You have no more active Gaming Groups. Please create one or re-activate one that is not currently Active."
                : message == ManageMessageId.EmptyGamingGroupName ? "You must enter a name for your Gaming Group."
                : "";

            SetToastMessage(TempMessageKeys.MANAGE_ACCOUNT_RESULT_TEMPMESSAGE, tempMessage, message.HasValue && message == ManageMessageId.Error ? "error" : "success");

            SetViewBag();
            var viewModel = GetBaseManageAccountViewModel();

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
            var gamingGroups = _gamingGroupRetriever.GetGamingGroupsForUser(currentUser.Id);
            var currentGamingGroup = gamingGroups.FirstOrDefault(gg => gg.Id == currentUser.CurrentGamingGroupId);
            if ((currentUser.CurrentGamingGroupId.HasValue && currentGamingGroup == null)
                || (currentUser.CurrentGamingGroupId == null && gamingGroups.Count > 0))
            {
                _gamingGroupContextSwitcher.EnsureContextIsValid(currentUser);
                currentGamingGroup = gamingGroups.FirstOrDefault(gg => gg.Id == currentUser.CurrentGamingGroupId);
            }

            var model = new UserGamingGroupsModel
            {
                GamingGroups = gamingGroups,
                CurrentGamingGroup = currentGamingGroup,
                CurrentUser = currentUser
            };

            return PartialView(MVC.Account.Views._UserGamingGroupsPartial, model);
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
                var result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
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
            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
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
            var result = await _userManager.SetEmailAsync(User.Identity.GetUserId(), model.EmailAddress.Trim());
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

        internal virtual ManageAccountViewModel GetBaseManageAccountViewModel()
        {
            var viewModel = new ManageAccountViewModel();
            var currentUserId = User.Identity.GetUserId();
            var user = _userManager.FindById(currentUserId);
            viewModel.PasswordViewModel = HasPassword() ? (PasswordViewModel)new ChangePasswordViewModel() : new SetPasswordViewModel();
            var emailViewModel = new ChangeEmailViewModel();
            emailViewModel.EmailAddress = user.Email;
            viewModel.ChangeEmailViewModel = emailViewModel;

            var userInformation = _userRetriever.RetrieveUserInformation(user);
            var bggUser = userInformation.BoardGameGeekUser;
            if (bggUser != null)
            {
                viewModel.BoardGameGeekIntegrationModel = new BoardGameGeekIntegrationModel
                {
                    BoardGameGeekUserName = bggUser.Name,
                    AvatarUrl = bggUser.Avatar,
                    IntegrationComplete = true,
                    BoardGameGeekUserUrl = BoardGameGeekUriBuilder.BuildBoardGameGeekUserUri(bggUser.Name)
                };
            }

            viewModel.GamingGroupsSummary = new GamingGroupsSummaryViewModel
            {
                ShowForEdit = true,
                GamingGroups = userInformation.GamingGroups.Select(x => _transformer.Transform<GamingGroupSummaryViewModel>(x)).ToList()
            };

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
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, true);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction(MVC.GamingGroup.Index());
                }

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
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
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
                var info = await _authenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser()
                {
                    UserName = model.UserName.Trim(),
                    Email = info.Email.Trim()
                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await _firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(user, TransactionSource.WebApplication);

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
            _authenticationManager.SignOut();
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
            var linkedAccounts = _userManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        [HttpPost]
        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult SetBoardGameGeekUser(BoardGameGeekIntegrationModel model, ApplicationUser currentUser)
        {
            var parentViewModel = GetBaseManageAccountViewModel();
            parentViewModel.BoardGameGeekIntegrationModel = model;

            if (ModelState.IsValid)
            {
                var bggResponse = _boardGameGeekApiClient.GetUserDetails(model.BoardGameGeekUserName);
                if (bggResponse == null)
                {
                    ModelState.AddModelError("BoardGameGeekUserName", "The user name given doesn't exist on BoardGameGeek");
                }
                else
                {
                    try
                    {
                        var saverResult = _boardGameGeekUserSaver.CreateUserDefintion(
                            new CreateBoardGameGeekUserDefinitionRequest()
                            {
                                Avatar = bggResponse.Avatar,
                                Name = bggResponse.Name,
                                BoardGameGeekUserId = bggResponse.UserId
                            }, currentUser);

                        if (saverResult != null)
                        {
                            return RedirectToAction(MVC.Account.ActionNames.Manage,
                                new { Message = ManageMessageId.SetBoardGameGeekUserSuccess });
                        }
                    }
                    catch (ArgumentException argumentException)
                    {
                        ModelState.AddModelError(nameof(model.BoardGameGeekUserName), argumentException.Message);
                    }
                }


            }
            SetViewBag();
            return View(MVC.Account.Views.Manage, parentViewModel);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
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
            var user = _userManager.FindById(User.Identity.GetUserId());
            return user?.PasswordHash != null;
        }

        public enum ManageMessageId
        {
            SetPasswordSuccess,
            SetBoardGameGeekUserSuccess,
            ChangePasswordSuccess,
            ChangeEmailSuccess,
            RemoveLoginSuccess,
            ErrorEnterEmail,
            Error,
            NoGamingGroup,
            EmptyGamingGroupName
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
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = passwordResetToken }, protocol: Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
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
            var result = await _userManager.ConfirmEmailAsync(userId, code);
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
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