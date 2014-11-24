using System;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public class UserRegisterer : IUserRegisterer
    {
        private readonly ApplicationUserManager applicationUserManager;
        private readonly IFirstTimeAuthenticator firstTimeUserAuthenticator;
        private readonly IDataContext dataContext;
        private readonly ApplicationSignInManager signInManager;
        private readonly INemeStatsEventTracker eventTracker;
        private readonly IGamingGroupInviteConsumer gamingGroupInviteConsumer;

        public UserRegisterer(
            ApplicationUserManager applicationUserManager, 
            IFirstTimeAuthenticator firstTimeUserAuthenticator, 
            IDataContext dataContext, 
            ApplicationSignInManager signInManager,
            INemeStatsEventTracker eventTracker, 
            IGamingGroupInviteConsumer gamingGroupInviteConsumer)
        {
            this.applicationUserManager = applicationUserManager;
            this.firstTimeUserAuthenticator = firstTimeUserAuthenticator;
            this.dataContext = dataContext;
            this.signInManager = signInManager;
            this.eventTracker = eventTracker;
            this.gamingGroupInviteConsumer = gamingGroupInviteConsumer;
        }

        public async Task<IdentityResult> RegisterUser(NewUser newUser)
        {
            ApplicationUser newApplicationUser = new ApplicationUser()
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                EmailConfirmed = true
            };

            IdentityResult identityResult = await applicationUserManager.CreateAsync(newApplicationUser, newUser.Password);

            if(identityResult.Succeeded)
            {
                new Task(() => eventTracker.TrackUserRegistration()).Start();
                await signInManager.SignInAsync(newApplicationUser, false, false);

                if (newUser.GamingGroupInvitationId.HasValue)
                {
                    gamingGroupInviteConsumer.AddNewUserToGamingGroup(newApplicationUser.Id, newUser.GamingGroupInvitationId.Value);
                }
                else
                {
                    await firstTimeUserAuthenticator.CreateGamingGroup(newApplicationUser);
                }
            }

            return identityResult;
        }
    }
}
