using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    //TODO make generic
    public abstract class EntityWithTechnicalKey<T> : EntityWithTechnicalKey, SingleColumnTechnicalKey<T>
    {
        public abstract T Id { get; set; }

        public virtual bool AlreadyInDatabase()
        {
            if(Id == null)
            {
                return false;
            }

            return !Id.Equals(default(T));
        }
    }

    public interface EntityWithTechnicalKey
    {
        bool AlreadyInDatabase();
    }
}
