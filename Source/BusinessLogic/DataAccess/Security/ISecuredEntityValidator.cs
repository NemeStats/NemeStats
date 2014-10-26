using BusinessLogic.Models.User;
using System;
using System.Linq;

namespace BusinessLogic.DataAccess.Security
{
    public interface ISecuredEntityValidator<TEntity> where TEntity : class
    {
        void ValidateAccess(TEntity entity, ApplicationUser currentUser, Type underlyingEntityType, object entityId);
    }
}
