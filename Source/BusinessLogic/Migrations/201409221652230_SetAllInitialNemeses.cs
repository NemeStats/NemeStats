namespace BusinessLogic.Migrations
{
    using BusinessLogic.DataAccess;
    using BusinessLogic.DataAccess.Repositories;
    using BusinessLogic.Logic.Nemeses;
    using System.Data.Entity.Migrations;
    
    public partial class SetAllInitialNemeses : DbMigration
    {
        public override void Up()
        {
            //since the model has changed, since this migration was originally written, this is no longer valid.
            //using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            //{
            //    using(NemeStatsDataContext dataContext = new NemeStatsDataContext())
            //    {
            //        IPlayerRepository playerRepository = new EntityFrameworkPlayerRepository(dataContext);
                    
            //        INemesisRecalculator nemesisRecalculator = new NemesisRecalculator(dataContext, playerRepository);
            //        nemesisRecalculator.RecalculateAllNemeses();
            //    }
            //}
        }
        
        public override void Down()
        {
        }
    }
}
