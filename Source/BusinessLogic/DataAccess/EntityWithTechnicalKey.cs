using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    //TODO make generic
    public abstract class EntityWithTechnicalKey : SingleColumnTechnicalKey
    {
        abstract public int Id { get; set; }

        public virtual bool AlreadyInDatabase()
        {
            return Id != default(int);
        }
    }
}
