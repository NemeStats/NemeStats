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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;

namespace BusinessLogic.DataAccess
{
    public class NemeStatsDataContext : IDataContext
    {
        internal const string CONNECTION_NAME = "DefaultConnection";
        internal const string UNKNOWN_ENTITY_ID = "<unknown>";
        internal const string EXCEPTION_MESSAGE_NO_ENTITY_EXISTS_FOR_THIS_ID = "No entity exists for Id '{0}'";

        private readonly SecuredEntityValidatorFactory securedEntityValidatorFactory;
        private readonly NemeStatsDbContext nemeStatsDbContext;

        //TODO do i really need this constructor? MockRepository.GenerateMock<ApplicationDbContext>() fails saying it needs a parameterless constructor
        public NemeStatsDataContext()
            : this(new NemeStatsDbContext(), new SecuredEntityValidatorFactory() )
        {

        }

        public NemeStatsDataContext(
            NemeStatsDbContext nemeStatsDbContext,
            SecuredEntityValidatorFactory securedEntityValidatorFactory)
        {
            this.nemeStatsDbContext = nemeStatsDbContext;
            this.securedEntityValidatorFactory = securedEntityValidatorFactory;
        }

        public virtual void CommitAllChanges()
        {
            nemeStatsDbContext.SaveChanges();
        }

        public virtual IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, EntityWithTechnicalKey
        {
            return nemeStatsDbContext.Set<TEntity>();
        }

        internal virtual TEntity AddOrInsertOverride<TEntity>(TEntity entity) where TEntity : class
        {
            nemeStatsDbContext.Set<TEntity>().AddOrUpdate(entity);

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
        public virtual TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey
        {
            ValidateArguments(entity, currentUser);

            var validator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>(this);
            //TODO how do I get this to be able to pull the Id from TEntity?
            validator.ValidateAccess(entity, currentUser, UNKNOWN_ENTITY_ID);

            if (!entity.AlreadyInDatabase())
            {
                SetGamingGroupIdIfEntityIsSecured(entity, currentUser);
            }

            var savedEntity = AddOrInsertOverride(entity);
            CommitAllChanges();

            return savedEntity;
        }

        private static void ValidateArguments<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser");
            }
        }

        private static void SetGamingGroupIdIfEntityIsSecured<TEntity>(TEntity entity, ApplicationUser currentUser) 
            where TEntity : class, EntityWithTechnicalKey
        {
            if (typeof(SecuredEntityWithTechnicalKey).IsAssignableFrom(typeof(TEntity)))
            {
                var securedEntity = entity as SecuredEntityWithTechnicalKey;
                if (securedEntity != null && securedEntity.GamingGroupId == default(int))
                {
                    securedEntity.GamingGroupId = currentUser.CurrentGamingGroupId;
                }
            }
        }

        public virtual void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey
        {
            var validator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>(this);
            validator.ValidateAccess(entity, currentUser, UNKNOWN_ENTITY_ID);
            nemeStatsDbContext.Set<TEntity>().Remove(entity);
        }

        public virtual DbRawSqlQuery<T> MakeRawSqlQuery<T>(string sql, params object[] parameters)
        {
            return nemeStatsDbContext.Database.SqlQuery<T>(sql, parameters);
        }

        public virtual void Dispose()
        {
            nemeStatsDbContext.Dispose();
        }

        public virtual void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey
        {
            var entityToDelete = FindById<TEntity>(id);

            var securedEntityValidator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>(this);
            securedEntityValidator.ValidateAccess(entityToDelete, currentUser, id);

            nemeStatsDbContext.Set<TEntity>().Remove(entityToDelete);
        }

        internal virtual void ValidateEntityExists<TEntity>(object id, TEntity entity) where TEntity : class, EntityWithTechnicalKey
        {
            if (entity == null)
            {
                throw new EntityDoesNotExistException(typeof(TEntity), id);
            }
        }

        public virtual TEntity FindById<TEntity>(object id) where TEntity : class, EntityWithTechnicalKey
        {
            var entity = nemeStatsDbContext.Set<TEntity>().Find(id);

            ValidateEntityExists(id, entity);

            return entity;
        }
    }
}
