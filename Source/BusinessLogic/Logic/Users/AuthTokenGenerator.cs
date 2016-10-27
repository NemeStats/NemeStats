using System;
using System.Configuration.Abstractions;
using System.Security.Cryptography;
using System.Text;
using BusinessLogic.DataAccess;
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
            string newAuthTokenString = GenerateNewAuthToken();
            var saltedHash = this.HashAuthToken(newAuthTokenString);

            ApplicationUser applicationUser = dataContext.FindById<ApplicationUser>(applicationUserId);
            applicationUser.AuthenticationToken = saltedHash;
            applicationUser.AuthenticationTokenExpirationDate = DateTime.UtcNow.AddMonths(3);

            dataContext.Save(applicationUser, applicationUser);

            return new AuthToken(newAuthTokenString, applicationUser.AuthenticationTokenExpirationDate);
        }

        internal virtual string GenerateNewAuthToken()
        {
            return Guid.NewGuid().ToString();
        }

        internal const int HASH_ITERATIONS = 1000;
        internal const int HASH_SIZE = 20;

        public virtual string HashAuthToken(string newAuthToken)
        {
            string salt = configManager.AppSettings[APP_KEY_AUTH_TOKEN_SALT];

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] authTokenBytes = Encoding.UTF8.GetBytes(newAuthToken);

            return System.Text.Encoding.Default.GetString(new Rfc2898DeriveBytes(authTokenBytes, saltBytes, HASH_ITERATIONS).GetBytes(HASH_SIZE));
        }
    }
}
