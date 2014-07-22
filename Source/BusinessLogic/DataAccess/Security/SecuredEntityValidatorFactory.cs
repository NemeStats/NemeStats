using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Security
{
    public class SecuredEntityValidatorFactory
    {
        public virtual SecuredEntityValidatorImpl<TEntity> MakeSecuredEntityValidator<TEntity>() where TEntity : class
        {
            //TODO this feels goofy.
            return new SecuredEntityValidatorImpl<TEntity>();
        }
    }
}
