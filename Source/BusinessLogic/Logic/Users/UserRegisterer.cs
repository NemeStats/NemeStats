using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public class UserRegisterer : IUserRegisterer
    {
        private ApplicationUserManager applicationUserManager;
        private IFirstTimeAuthenticator firstTimeUserAuthenticator;

        public UserRegisterer(ApplicationUserManager applicationUserManager, IFirstTimeAuthenticator firstTimeUserAuthenticator)
        {
            this.applicationUserManager = applicationUserManager;
            this.firstTimeUserAuthenticator = firstTimeUserAuthenticator;
        }

        public async Task<IdentityResult> RegisterUser(NewUser newUser)
        {
            ApplicationUser newApplicationUser = new ApplicationUser()
            {
                UserName = newUser.UserName,
                Email = newUser.Email
            };

            IdentityResult identityResult = await applicationUserManager.CreateAsync(newApplicationUser, newUser.Password);

            if(identityResult.Succeeded)
            {
                await firstTimeUserAuthenticator.SignInAndCreateGamingGroup(newApplicationUser);
            }

            return identityResult;
        }
    }
}
