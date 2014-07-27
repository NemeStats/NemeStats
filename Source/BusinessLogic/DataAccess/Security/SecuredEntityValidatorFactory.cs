using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Security
{
    public class SecuredEntityValidatorFactory
    {
        public virtual SecuredEntityValidator<TEntity> MakeSecuredEntityValidator<TEntity>() where TEntity : class
        {
            //TODO this feels goofy. Should I call StructureMap directly here?
            return new SecuredEntityValidatorImpl<TEntity>();
        }
    }
}
