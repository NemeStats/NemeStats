using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using BusinessLogic.Models.User;

namespace UI.Attributes
{
    public class ClientIdCalculator
    {
        /// <summary>
        /// The parameter key for the Google Analytics unique client id.
        /// </summary>
        internal const string COOKIE_NAME_GOOGLE_ANALYTICS = "_ga";

        public virtual string GetClientId(System.Web.HttpRequestBase httpRequestBase, ApplicationUser applicationUser)
        {
            var cookie = httpRequestBase.Cookies[COOKIE_NAME_GOOGLE_ANALYTICS];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return applicationUser.Id;
            }

            return cookie.Value;
        }

        public virtual string GetClientId(HttpRequestMessage httpRequestBase, ApplicationUser applicationUser)
        {
            CookieHeaderValue cookie = httpRequestBase.Headers.GetCookies(COOKIE_NAME_GOOGLE_ANALYTICS).FirstOrDefault();
            return cookie != null ? cookie[COOKIE_NAME_GOOGLE_ANALYTICS].Value : applicationUser.Id;
        }
    }
}