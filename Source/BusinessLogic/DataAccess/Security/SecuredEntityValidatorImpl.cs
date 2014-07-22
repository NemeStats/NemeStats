using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Security
{
    public class SecuredEntityValidatorImpl<TEntity> : SecuredEntityValidator<TEntity> where TEntity : class
    {
        internal const string EXCEPTION_MESSAGE_USER_DOES_NOT_HAVE_ACCESS_TO_GAME_DEFINITION
           = "User with Id '{0} is unauthorized to access '{1}' with Id '{2}'";

        public void ValidateAccess(SecuredEntity securedEntity, ApplicationUser currentUser, Type entityType)
        {
            throw new NotImplementedException();
        }
    }
}
