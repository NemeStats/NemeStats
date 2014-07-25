using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace BusinessLogic.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, DataContext
    {
        internal const string CONNECTION_NAME = "DefaultConnection";
        internal const string EXCEPTION_MESSAGE_CURRENT_GAMING_GROUP_ID_CANNOT_BE_NULL = "currentUser.CurrentGamingGroupId cannot be null";
        private SecuredEntityValidatorFactory securedEntityValidatorFactory;

        //TODO do i really need this constructor? MockRepository.GenerateMock<ApplicationDbContext>() fails saying it needs a parameterless constructor
        public ApplicationDbContext() 
            : this(new SecuredEntityValidatorFactory())
        {

        }

        public ApplicationDbContext(SecuredEntityValidatorFactory securedEntityValidatorFactory)
            : base(CONNECTION_NAME)
        {
            this.securedEntityValidatorFactory = securedEntityValidatorFactory;
        }

        public virtual DbSet<GamingGroup> GamingGroups { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<GameDefinition> GameDefinitions { get; set; }
        public virtual DbSet<PlayedGame> PlayedGames { get; set; }
        public virtual DbSet<PlayerGameResult> PlayerGameResults { get; set; }
        public virtual DbSet<UserGamingGroup> UserGamingGroups { get; set; }
        public virtual DbSet<GamingGroupInvitation> GamingGroupInvitations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public virtual void CommitAllChanges()
        {
            SaveChanges();
        }

        public virtual IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        internal virtual TEntity AddOrInsertOverride<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().AddOrUpdate(entity);

            return entity;
        }

        public virtual TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey
        {
            ValidateArguments<TEntity>(entity, currentUser);

            if (entity.AlreadyInDatabase())
            {
                //TODO update comments to indicate it can throw an exception
                SecuredEntityValidator<TEntity> validator = securedEntityValidatorFactory.MakeSecuredEntityValidator<TEntity>();
                validator.ValidateAccess(entity, currentUser, typeof(TEntity));
            }
            else
            {
                //TODO refactor this out into another method
                SetGamingGroupIdIfEntityIsSecured<TEntity>(entity, currentUser);
            }

            TEntity savedEntity = AddOrInsertOverride<TEntity>(entity);

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

            if (currentUser.CurrentGamingGroupId == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_CURRENT_GAMING_GROUP_ID_CANNOT_BE_NULL);
            }
        }

        private static void SetGamingGroupIdIfEntityIsSecured<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey
        {
            if (entity is SecuredEntityWithTechnicalKey)
            {
                SecuredEntityWithTechnicalKey securedEntity = entity as SecuredEntityWithTechnicalKey;
                securedEntity.GamingGroupId = currentUser.CurrentGamingGroupId.Value;
            }
        }
    }
}
