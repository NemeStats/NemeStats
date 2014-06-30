
namespace BusinessLogic.DataAccess
{
    public class NemeStatsInitializer : System.Data.Entity.CreateDatabaseIfNotExists<NemeStatsDbContext>
    {
        protected override void Seed(NemeStatsDbContext context)
        {
            //DataSeeder dataSeeder = new DataSeeder(context);
            //dataSeeder.SeedData();
        }
    }
}
