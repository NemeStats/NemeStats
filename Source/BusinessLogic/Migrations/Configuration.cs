namespace BusinessLogic.Migrations
{
    using BusinessLogic.Logic;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BusinessLogic.DataAccess.NerdScorekeeperDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BusinessLogic.DataAccess.NerdScorekeeperDbContext context)
        {
            new DataSeeder(context).SeedData();
        }
    }
}
