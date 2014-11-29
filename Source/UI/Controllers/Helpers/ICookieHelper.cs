using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UI.Controllers.Helpers
{
    public interface ICookieHelper
    {
        void ClearCookie(NemeStatsCookieEnum nemeStatsCookieEnum, HttpRequestBase httpRequestBase, HttpResponseBase httpResponseBase);
    }
}
