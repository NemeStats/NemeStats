using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repository
{
    public interface UnitOfWork
    {
        int SaveChanges();
    }
}
