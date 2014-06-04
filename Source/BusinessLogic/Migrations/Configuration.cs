namespace BusinessLogic.Migrations
{
    using BusinessLogic.Logic;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BusinessLogic.DataAccess.NemeStatsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BusinessLogic.DataAccess.NemeStatsDbContext context)
        {
            new DataSeeder(context).SeedData();
        }
    }
}
