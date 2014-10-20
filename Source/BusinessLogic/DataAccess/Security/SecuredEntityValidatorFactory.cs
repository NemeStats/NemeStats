using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Security
{
    public class SecuredEntityValidatorFactory
    {
        public virtual ISecuredEntityValidator<TEntity> MakeSecuredEntityValidator<TEntity>() where TEntity : class
        {
            //TODO this feels goofy. Should I call StructureMap directly here?
            //TODO possibly dish out a singleton since there  only needs to exist one of these per TEntity
            return new SecuredEntityValidator<TEntity>();
        }
    }
}
