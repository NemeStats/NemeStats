using BusinessLogic.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace UI.Logic
{
    public interface UserContextBuilder
    {
        UserContext GetUserContext(HttpRequestBase request, IIdentity userIdentity);
    }
}