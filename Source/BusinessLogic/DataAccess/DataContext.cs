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
        IQueryable<TEntity> GetQueryable<TEntity>(ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey;

        TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey;
        void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey;
        void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey;
    }
}
