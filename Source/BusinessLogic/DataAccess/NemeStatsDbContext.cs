using BusinessLogic.Models;
using BusinessLogic.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public class NemeStatsDbContext : IdentityDbContext<ApplicationUser>
    {
        public NemeStatsDbContext()
            : base("DefaultConnection")
        {
        }

        public virtual DbSet<GamingGroup> GamingGroups { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<GameDefinition> GameDefinitions { get; set; }
        public virtual DbSet<PlayedGame> PlayedGames { get; set; }
        public virtual DbSet<PlayerGameResult> PlayerGameResults { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
