#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.Models.User;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BusinessLogic.DataAccess
{
    public interface IDataContext : IDisposable
    {
        void CommitAllChanges();
        DbRawSqlQuery<T> MakeRawSqlQuery<T>(string sql, params object[] parameters);
        IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntityWithTechnicalKey;
        TEntity FindById<TEntity>(object id) where TEntity : class, IEntityWithTechnicalKey;
        TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey;
        void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey;
        void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey;
        DbContextTransaction CurrentTransaction();
        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        /// <summary>
        /// Makes sure that the data context clears out any cache of entities of type TEntity so that the next query will pull them from the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        void DetachEntities<TEntity>() where TEntity : class, IEntityWithTechnicalKey;
        /// <summary>
        /// Saves without checking security. Use carefully.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity AdminSave<TEntity>(TEntity entity) where TEntity : class, IEntityWithTechnicalKey;

        void SetCommandTimeout(int timeoutInSeconds);
    }
}
