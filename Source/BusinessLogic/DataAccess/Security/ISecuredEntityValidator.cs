using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Security
{
    public interface ISecuredEntityValidator<TEntity> where TEntity : class
    {
        void ValidateAccess(TEntity entity, ApplicationUser currentUser, Type underlyingEntityType, object entityId);
    }
}
