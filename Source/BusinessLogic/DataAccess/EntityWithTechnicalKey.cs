using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public abstract class EntityWithTechnicalKey : SingleColumnTechnicalKey
    {
        abstract public int Id { get; set; }

        public virtual bool AlreadyInDatabase()
        {
            return Id != 0;
        }
    }
}
