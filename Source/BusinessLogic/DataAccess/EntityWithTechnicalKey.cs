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

        public override bool AlreadyInDatabase()
        {
            if(Id == null)
            {
                return false;
            }

            return Id.Equals(default(T));
        }
    }

    public abstract class EntityWithTechnicalKey
    {
        public abstract bool AlreadyInDatabase();
    }
}
