using BusinessLogic.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace UI.Logic
{
    public class UserContextBuilderImpl : UserContextBuilder
    {
        UserContext UserContextBuilder.GetUserContext(HttpRequestBase request, IIdentity userIdentity)
        {
            throw new NotImplementedException();
        }
    }
}
