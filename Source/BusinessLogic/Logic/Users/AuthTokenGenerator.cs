using System;
using System.Configuration.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public class AuthTokenGenerator : IAuthTokenGenerator
    {
        internal const string APP_KEY_AUTH_TOKEN_SALT = "authTokenSalt";
        private readonly IDataContext dataContext;
        private readonly IConfigurationManager configManager;

        public AuthTokenGenerator(IDataContext dataContext, IConfigurationManager configManager)
        {
            this.dataContext = dataContext;
            this.configManager = configManager;
        }

        public AuthToken GenerateAuthToken(string applicationUserId, string uniqueDeviceId = null)
        {
            var newAuthTokenString = GenerateNewAuthToken();
            var saltedHash = HashAuthToken(newAuthTokenString);

            var applicationUser = dataContext.FindById<ApplicationUser>(applicationUserId);

            var userDeviceAuthToken = dataContext.GetQueryable<UserDeviceAuthToken>()
                .FirstOrDefault(x => x.ApplicationUserId == applicationUserId && x.DeviceId == uniqueDeviceId);

            if (userDeviceAuthToken == null)
            {
                userDeviceAuthToken = new UserDeviceAuthToken
                {
                    ApplicationUserId = applicationUserId,
                    DeviceId = uniqueDeviceId
                };
            }

            userDeviceAuthToken.AuthenticationToken = saltedHash;
            userDeviceAuthToken.AuthenticationTokenExpirationDate = DateTime.UtcNow.AddMonths(3);

            dataContext.Save(userDeviceAuthToken, applicationUser);

            return new AuthToken(newAuthTokenString, userDeviceAuthToken.AuthenticationTokenExpirationDate);
        }

        internal virtual string GenerateNewAuthToken()
        {
            return Guid.NewGuid().ToString();
        }

        internal const int HASH_ITERATIONS = 1000;
        internal const int HASH_SIZE = 20;

        public virtual string HashAuthToken(string newAuthToken)
        {
            var salt = configManager.AppSettings[APP_KEY_AUTH_TOKEN_SALT];

            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var authTokenBytes = Encoding.UTF8.GetBytes(newAuthToken);

            return Encoding.Default.GetString(new Rfc2898DeriveBytes(authTokenBytes, saltBytes, HASH_ITERATIONS).GetBytes(HASH_SIZE));
        }
    }
}
