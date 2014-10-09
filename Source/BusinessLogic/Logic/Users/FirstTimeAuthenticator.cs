using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.User;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public class FirstTimeAuthenticator : IFirstTimeAuthenticator
    {
        private IAuthenticationManager authenticationManager;
        private INemeStatsEventTracker eventTracker;
        private ApplicationSignInManager signInManager;
        private IGamingGroupInviteConsumer gamingGroupInviteConsumer;
        private IGamingGroupSaver gamingGroupSaver;

        public FirstTimeAuthenticator(
            IAuthenticationManager authenticationManager, 
            INemeStatsEventTracker eventTracker,
            ApplicationSignInManager signInManager,
            IGamingGroupInviteConsumer gamingGroupInviteConsumer,
            IGamingGroupSaver gamingGroupSaver)
        {
            this.authenticationManager = authenticationManager;
            this.eventTracker = eventTracker;
            this.signInManager = signInManager;
            this.gamingGroupInviteConsumer = gamingGroupInviteConsumer;
            this.gamingGroupSaver = gamingGroupSaver;
        }

        public async Task<object> SignInAndCreateGamingGroup(ApplicationUser applicationUser)
        {
            new Task(() => eventTracker.TrackUserRegistration()).Start();

            await signInManager.SignInAsync(applicationUser, false, false);
            int? gamingGroupIdToWhichTheUserWasAdded = await gamingGroupInviteConsumer.ConsumeGamingGroupInvitation(applicationUser);

            if (!gamingGroupIdToWhichTheUserWasAdded.HasValue)
            {
                await gamingGroupSaver.CreateNewGamingGroup(applicationUser.UserName + "'s Gaming Group", applicationUser);
            }

            return new object();
        }
    }
}
