using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
