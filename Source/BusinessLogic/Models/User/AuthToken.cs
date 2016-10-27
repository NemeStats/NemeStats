using System;

namespace BusinessLogic.Models.User
{
    public class AuthToken
    {
        public string AuthenticationTokenString { get; set; }
        public DateTime AuthenticationTokenExpirationDateTime { get; set; }

        public AuthToken(string authenticationTokenString, DateTime authenticationTokenExpirationDateTime)
        {
            AuthenticationTokenString = authenticationTokenString;
            AuthenticationTokenExpirationDateTime = authenticationTokenExpirationDateTime;
        }
    }
}