using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public interface DataContext : IDisposable
    {
        void CommitAllChanges();

        IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class;

        TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey;
    }
}
