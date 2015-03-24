using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.Users
{
    public interface IAuthTokenGenerator
    {
        string GenerateAuthToken(string applicationUserId);
    }
}
