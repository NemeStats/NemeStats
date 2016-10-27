using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using System;
using BusinessLogic.Models;

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

        public UserDeviceAuthToken ValidateAuthToken(string authToken)
        {
            string hashedAndSaltedAuthToken = authTokenGenerator.HashAuthToken(authToken);
            return dataContext.GetQueryable<UserDeviceAuthToken>()
                .FirstOrDefault(x => x.AuthenticationToken == hashedAndSaltedAuthToken && DateTime.UtcNow <= x.AuthenticationTokenExpirationDate);
        }
    }
}
