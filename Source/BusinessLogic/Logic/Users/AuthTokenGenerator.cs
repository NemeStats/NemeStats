using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
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

        public string GenerateAuthToken(string applicationUserId)
        {
            string salt = configManager.AppSettings[APP_KEY_AUTH_TOKEN_SALT];
            string newAuthToken = GenerateNewAuthToken();
            var saltedHash = GetNewSaltedHashedAuthenticationToken(salt, newAuthToken);

            ApplicationUser applicationUser = dataContext.FindById<ApplicationUser>(applicationUserId);
            applicationUser.AuthenticationToken = saltedHash;
            applicationUser.AuthenticationTokenExpirationDate = DateTime.UtcNow.AddMonths(3);

            dataContext.Save(applicationUser, applicationUser);

            return newAuthToken;
        }

        internal virtual string GenerateNewAuthToken()
        {
            return Guid.NewGuid().ToString();
        }

        internal virtual string GetNewSaltedHashedAuthenticationToken(string salt, string newAuthToken)
        {
            byte[] saltBytes = new byte[salt.Length * sizeof(char)];
            Buffer.BlockCopy(salt.ToCharArray(), 0, saltBytes, 0, saltBytes.Length);

            var hmacMd5 = new HMACMD5(saltBytes);
            byte[] authTokenBytes = new byte[newAuthToken.Length * sizeof(char)];
            Buffer.BlockCopy(newAuthToken.ToCharArray(), 0, authTokenBytes, 0, authTokenBytes.Length);
            var saltedHash = hmacMd5.ComputeHash(authTokenBytes);
            return saltedHash.ToString();
        }
    }
}
