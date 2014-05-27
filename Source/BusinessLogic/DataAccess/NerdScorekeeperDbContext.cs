using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public class NerdScorekeeperDbContext : DbContext
    {
        public NerdScorekeeperDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<GameDefinition> GameDefinitions { get; set; }
        public DbSet<PlayedGame> PlayedGames { get; set; }
        public DbSet<PlayerGameResult> PlayerGameResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
