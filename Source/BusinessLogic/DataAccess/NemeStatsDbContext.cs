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

using System;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using BusinessLogic.Models.Games;
using Microsoft.Azure;

namespace BusinessLogic.DataAccess
{
    public class NemeStatsDbContext : IdentityDbContext<ApplicationUser>
    {
        internal const string CONNECTION_STRING_KEY = "Database.ConnectionString";

        public NemeStatsDbContext() : base(CloudConfigurationManager.GetSetting(CONNECTION_STRING_KEY))
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;

            //uncomment to turn on SQL statements printing to the console
            //this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public virtual DbSet<GamingGroup> GamingGroups { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<GameDefinition> GameDefinitions { get; set; }
        public virtual DbSet<BoardGameGeekGameDefinition> BoardGameGeekGameDefinitions { get; set; }
        public virtual DbSet<BoardGameGeekUserDefinition> BoardGameGeekUserDefinitions { get; set; }
        public virtual DbSet<PlayedGame> PlayedGames { get; set; }
        public virtual DbSet<PlayerGameResult> PlayerGameResults { get; set; }
        public virtual DbSet<PlayedGameApplicationLinkage> PlayedGameApplicationLinkages { get; set; }
        public virtual DbSet<UserGamingGroup> UserGamingGroups { get; set; }
        public virtual DbSet<GamingGroupInvitation> GamingGroupInvitations { get; set; }
        public virtual DbSet<Nemesis> Nemeses { get; set; }
        public virtual DbSet<Champion> Champions { get; set; }
        public virtual DbSet<VotableFeature> VotableFeatures { get; set; }
        public virtual DbSet<BoardGameGeekGameCategory> BoardGameGeekGameCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<GamingGroupInvitation>()
            .HasRequired(i => i.RegisteredPlayer)
            .WithMany(p => p.Invitations)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<PlayerAchievement>()
             .HasRequired(i => i.Player)
            .WithMany(p => p.PlayerAchievements)
            .WillCascadeOnDelete(true);

            modelBuilder.Entity<BoardGameGeekGameDefinition>()
            .HasMany(v => v.Categories)
            .WithMany(p => p.Games)
          .Map(
            m =>
        {
            m.MapLeftKey("Id");
            m.MapRightKey("BoardGameGeekGameCategoryId");
            m.ToTable("BoardGameGeekGameToCategory");
        });

            base.OnModelCreating(modelBuilder);
        }
    }
}
