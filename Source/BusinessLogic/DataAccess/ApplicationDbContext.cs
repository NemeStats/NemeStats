using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace BusinessLogic.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, DataContext
    {
        internal const string CONNECTION_NAME = "DefaultConnection";
        private SecuredEntityValidator securedEntityValidator;

        public ApplicationDbContext(SecuredEntityValidator securedEntityValidator)
            : base(CONNECTION_NAME)
        {
            this.securedEntityValidator = securedEntityValidator;
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

        public virtual TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : EntityWithTechnicalKey
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
                throw new ArgumentException("currentUser.CurrentGamingGroupId cannot be null");
            }

            if (entity.AlreadyInDatabase())
            {
                //TODO update comments to indicate it can throw an exception
                securedEntityValidator.ValidateAccess(entity, currentUser, typeof(TEntity));
                Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                //TODO refactor this out into another method
                if (entity is SecuredEntity)
                {
                    SecuredEntity securedEntity = (SecuredEntity)entity;
                    securedEntity.GamingGroupId = currentUser.CurrentGamingGroupId.Value;
                }
                Set<TEntity>().Add(entity);
            }

            return entity;
        }
    }
}
