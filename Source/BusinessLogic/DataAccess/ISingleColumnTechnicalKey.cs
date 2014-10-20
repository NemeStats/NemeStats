using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public interface ISingleColumnTechnicalKey<T>
    {
        T Id { get; set; }
    }
}
