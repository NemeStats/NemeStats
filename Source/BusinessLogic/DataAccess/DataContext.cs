using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public interface DataContext : IDisposable
    {
        void CommitAllChanges();
        DbRawSqlQuery<T> MakeRawSqlQuery<T>(string sql, params object[] parameters);
        IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, EntityWithTechnicalKey;
        TEntity FindById<TEntity>(object id) where TEntity : class, EntityWithTechnicalKey;
        TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey;
        void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey;
        void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey;
    }
}
