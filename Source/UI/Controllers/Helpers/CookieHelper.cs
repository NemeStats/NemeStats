using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Controllers.Helpers
{
    public class CookieHelper : ICookieHelper
    {
        public void ClearCookie(NemeStatsCookieEnum nemeStatsCookieEnum, HttpRequestBase httpRequestBase, HttpResponseBase httpResponseBase)
        {
            string cookieName = nemeStatsCookieEnum.ToString();
            if (httpRequestBase.Cookies[cookieName] != null)
            {
                HttpCookie myCookie = new HttpCookie(cookieName)
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                httpResponseBase.Cookies.Add(myCookie);
            }
        }
    }
}