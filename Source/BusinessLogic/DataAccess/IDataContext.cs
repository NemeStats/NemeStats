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
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.DataAccess
{
    public interface IDataContext : IDisposable
    {
        void CommitAllChanges();
        DbRawSqlQuery<T> MakeRawSqlQuery<T>(string sql, params object[] parameters);
        IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, EntityWithTechnicalKey;
        TEntity FindById<TEntity>(object id) where TEntity : class, EntityWithTechnicalKey;
        TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser, List<IBusinessLogicEvent> events = null) where TEntity : class, EntityWithTechnicalKey;
        void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey;
        void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey;
    }
}
