using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using System;

namespace BusinessLogic.Logic.Users
{
    public class AuthTokenValidator : IAuthTokenValidator
    {
        private readonly IAuthTokenGenerator authTokenGenerator;
        private readonly IDataContext dataContext;

        public AuthTokenValidator(IAuthTokenGenerator authTokenGenerator, IDataContext dataContext)
        {
            this.authTokenGenerator = authTokenGenerator;
            this.dataContext = dataContext;
        }

        public ApplicationUser ValidateAuthToken(string authToken)
        {
            string hashedAndSaltedAuthToken = authTokenGenerator.HashAuthToken(authToken);
            return dataContext.GetQueryable<ApplicationUser>()
                .FirstOrDefault(x => x.UserDeviceAuthTokens.Any(y => y.AuthenticationToken == hashedAndSaltedAuthToken && DateTime.UtcNow <= y.AuthenticationTokenExpirationDate));
        }
    }
}
