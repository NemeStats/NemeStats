using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface WriteEntities : ReadEntities, UnitOfWork
    {
        IQueryable<TEntity> Load<TEntity>();
        void Create<TEntity>(TEntity entity);
        void Update<TEntity>(TEntity entity);
        void Delete<TEntity>(TEntity entity);
    }
}
