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
        internal const string EXCEPTION_MESSAGE_CURRENT_GAMING_GROUP_ID_CANNOT_BE_NULL = "currentUser.CurrentGamingGroupId cannot be null";
        internal const string EXCEPTION_MESSAGE_NO_ENTITY_EXISTS_FOR_THIS_ID = "No entity exists for Id '{0}'";

        private readonly SecuredEntityValidatorFactory securedEntityValidatorFactory;
        private readonly NemeStatsDbContext nemeStatsDbContext;

        //TODO do i really need this constructor? MockRepository.GenerateMock<ApplicationDbContext>() fails saying it needs a parameterless constructor
        public NemeStatsDataContext()
            : this(new NemeStatsDbContext(), new SecuredEntityValidatorFactory())
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
        //TODO If the passed in TEntity that is new, the Id will not be set until SaveChanges is called
        public virtual TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey
        {
            ValidateArguments<TEntity>(entity, currentUser);

            if (entity.AlreadyInDatabase())
            {
                //TODO update comments to indicate it can throw an exception
                ISecuredEntityValidator<TEntity> validator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>();
                //TODO how do I get this to be able to pull the Id from TEntity?
                validator.ValidateAccess(entity, currentUser, typeof(TEntity), UNKNOWN_ENTITY_ID);
            }
            else
            {
                SetGamingGroupIdIfEntityIsSecured<TEntity>(entity, currentUser);
            }

            TEntity savedEntity = AddOrInsertOverride<TEntity>(entity);
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
                if(currentUser.CurrentGamingGroupId == null)
                {
                    throw new ArgumentException(EXCEPTION_MESSAGE_CURRENT_GAMING_GROUP_ID_CANNOT_BE_NULL);
                }
                SecuredEntityWithTechnicalKey securedEntity = entity as SecuredEntityWithTechnicalKey;
                securedEntity.GamingGroupId = currentUser.CurrentGamingGroupId.Value;
            }
        }

        public virtual void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey
        {
            ISecuredEntityValidator<TEntity> validator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>();
            validator.ValidateAccess(entity, currentUser, typeof(TEntity), UNKNOWN_ENTITY_ID);
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
            TEntity entityToDelete = FindById<TEntity>(id);

            ISecuredEntityValidator<TEntity> securedEntityValidator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>();
            securedEntityValidator.ValidateAccess(entityToDelete, currentUser, typeof(TEntity), id);

            nemeStatsDbContext.Set<TEntity>().Remove(entityToDelete);
        }

        internal virtual void ValidateEntityExists<TEntity>(object id, TEntity entity) where TEntity : class, EntityWithTechnicalKey
        {
            if (entity == null)
            {
                throw new EntityDoesNotExistException(id);
            }
        }

        public virtual TEntity FindById<TEntity>(object id) where TEntity : class, EntityWithTechnicalKey
        {
            TEntity entity = nemeStatsDbContext.Set<TEntity>().Find(id);

            ValidateEntityExists(id, entity);

            return entity;
        }
    }
}
