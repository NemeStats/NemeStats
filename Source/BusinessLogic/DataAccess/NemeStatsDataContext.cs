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
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Exceptions;
using BusinessLogic.Models.User;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using BusinessLogic.Models;

namespace BusinessLogic.DataAccess
{
    public class NemeStatsDataContext : IDataContext
    {
        internal const string CONNECTION_NAME = "DefaultConnection";
        internal const string UNKNOWN_ENTITY_ID = "<unknown>";
        internal const string EXCEPTION_MESSAGE_NO_ENTITY_EXISTS_FOR_THIS_ID = "No entity exists for Id '{0}'";

        private readonly ISecuredEntityValidatorFactory _securedEntityValidatorFactory;
        private readonly NemeStatsDbContext _nemeStatsDbContext;

        //TODO do i really need this constructor? MockRepository.GenerateMock<ApplicationDbContext>() fails saying it needs a parameterless constructor
        public NemeStatsDataContext()
            : this(new NemeStatsDbContext(), new SecuredEntityValidatorFactory() )
        {

        }

        public NemeStatsDataContext(
            NemeStatsDbContext nemeStatsDbContext,
            ISecuredEntityValidatorFactory securedEntityValidatorFactory)
        {
            _nemeStatsDbContext = nemeStatsDbContext;
            _securedEntityValidatorFactory = securedEntityValidatorFactory;
        }

        public virtual void CommitAllChanges()
        {
            _nemeStatsDbContext.SaveChanges();
        }

        public virtual IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntityWithTechnicalKey
        {
            return _nemeStatsDbContext.Set<TEntity>();
        }

        internal virtual TEntity AddOrInsertOverride<TEntity>(TEntity entity) where TEntity : class
        {
            _nemeStatsDbContext.Set<TEntity>().AddOrUpdate(entity);

            return entity;
        }

        /// <summary>
        /// Attempts to save the given entity while ensuring that the given user has the appropriate security to save the given entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="currentUser"></param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not allowed to save this entity because the user does not have access to the gaming group.</exception>
        /// <returns></returns>
        public virtual TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey
        {
            ValidateArguments(entity, currentUser);

            var validator = _securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>(this);
            validator.ValidateAccess(entity, currentUser);

            if (!entity.AlreadyInDatabase() && typeof(TEntity) != typeof(GamingGroup))
            {
                SetGamingGroupIdIfEntityIsSecured(entity, currentUser);
            }

            var savedEntity = AddOrInsertOverride(entity);
            CommitAllChanges();

            return savedEntity;
        }

        private static void ValidateArguments<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : IEntityWithTechnicalKey
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }
        }

        internal virtual void SetGamingGroupIdIfEntityIsSecured<TEntity>(TEntity entity, ApplicationUser currentUser) 
            where TEntity : class, IEntityWithTechnicalKey
        {
            if (typeof(SecuredEntityWithTechnicalKey).IsAssignableFrom(typeof(TEntity)))
            {
                if (!currentUser.CurrentGamingGroupId.HasValue)
                {
                    throw new UserHasNoGamingGroupException(currentUser.Id);
                }
                var securedEntity = entity as SecuredEntityWithTechnicalKey;
                if (securedEntity != null && securedEntity.GamingGroupId == default(int))
                {
                    securedEntity.GamingGroupId = currentUser.CurrentGamingGroupId.Value;
                }
            }
        }

        public virtual void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey
        {
            var validator = _securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>(this);
            validator.ValidateAccess<TEntity>(entity, currentUser);
            _nemeStatsDbContext.Set<TEntity>().Remove(entity);
        }

        public virtual DbRawSqlQuery<T> MakeRawSqlQuery<T>(string sql, params object[] parameters)
        {
            return _nemeStatsDbContext.Database.SqlQuery<T>(sql, parameters);
        }

        public virtual void MakeScarySqlAlteration(string sql, params object[] parameters)
        {
            _nemeStatsDbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

        public DbContextTransaction CurrentTransaction()
        {
            return _nemeStatsDbContext.Database.CurrentTransaction;
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return _nemeStatsDbContext.Database.BeginTransaction(isolationLevel);
        }

        public virtual void Dispose()
        {
            _nemeStatsDbContext.Dispose();
        }

        public virtual void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey
        {
            var securedEntityValidator = _securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>(this);
            var entityToDelete = securedEntityValidator.RetrieveAndValidateAccess<TEntity>(id, currentUser);

            _nemeStatsDbContext.Set<TEntity>().Remove(entityToDelete);
        }

        internal virtual void ValidateEntityExists<TEntity>(object id, TEntity entity) where TEntity : class, IEntityWithTechnicalKey
        {
            if (entity == null)
            {
                throw new EntityDoesNotExistException<TEntity>(id);
            }
        }

        /// <exception cref="EntityDoesNotExistException{T}">Thrown when the specified entity id does not match a real entity.</exception>
        public virtual TEntity FindById<TEntity>(object id) where TEntity : class, IEntityWithTechnicalKey
        {
            var entity = _nemeStatsDbContext.Set<TEntity>().Find(id);

            ValidateEntityExists(id, entity);

            return entity;
        }

        public void DetachEntities<TEntity>() where TEntity : class, IEntityWithTechnicalKey
        {
            var trackedEntities = _nemeStatsDbContext.ChangeTracker.Entries<TEntity>().ToList();
            foreach (var entity in trackedEntities)
            {
                entity.State = EntityState.Detached;
            }
        }

        /// <exception cref="ArgumentNullException">Thrown when the passed in entity is null.</exception>
        /// <exception cref="EntityDoesNotExistException{T}">Thrown when the passed in entityis a SecuredEntityWithTechnical key, is not a GamingGroup, and doesn't have a GamingGroupId set.</exception>
        public TEntity AdminSave<TEntity>(TEntity entity) where TEntity : class, IEntityWithTechnicalKey
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var securedEntity = entity as SecuredEntityWithTechnicalKey;
            if (securedEntity != null && securedEntity.GamingGroupId == default(int))
            {
                throw new ArgumentException("GamingGroupId must be set on an ISecuredEntityWithTechnicalKey");
            }

            var savedEntity = AddOrInsertOverride(entity);
            CommitAllChanges();

            return savedEntity;
        }

        public void SetCommandTimeout(int timeoutInSeconds)
        {
            _nemeStatsDbContext.Database.CommandTimeout = timeoutInSeconds;
        }
    }
}
