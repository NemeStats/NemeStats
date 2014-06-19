
namespace BusinessLogic.DataAccess
{
    public class NemeStatsInitializer : System.Data.Entity.CreateDatabaseIfNotExists<NemeStatsDbContext>
    {
        //TODO review with Clean Code book club
        protected override void Seed(NemeStatsDbContext context)
        {
            //TODO Seed should only run after hte database has been created for the first time (but after all migrations have run), not every time a migration is applied
            //DataSeeder dataSeeder = new DataSeeder(context);
            //dataSeeder.SeedData();
        }
    }
}
