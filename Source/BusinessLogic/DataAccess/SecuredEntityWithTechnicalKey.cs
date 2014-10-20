using System.Linq;

namespace BusinessLogic.DataAccess
{
    public abstract class SecuredEntityWithTechnicalKey<T> : EntityWithTechnicalKey<T>, SecuredEntityWithTechnicalKey
    {
        public override T Id { get; set; }
        public virtual int GamingGroupId { get; set; }
    }

    public interface SecuredEntityWithTechnicalKey
    {
        int GamingGroupId { get; set; }
    }
}
