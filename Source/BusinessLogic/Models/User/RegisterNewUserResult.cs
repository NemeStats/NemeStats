using Microsoft.AspNet.Identity;

namespace BusinessLogic.Models.User
{
    public class RegisterNewUserResult
    {
        public IdentityResult Result { get; set; }

        public NewlyRegisteredUser NewlyRegisteredUser { get; set; }
    }
}
