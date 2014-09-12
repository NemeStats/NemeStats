using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public interface SingleColumnTechnicalKey<T>
    {
        T Id { get; set; }
    }
}
