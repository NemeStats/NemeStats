using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.Champions;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecalculateChampions_Again : DbMigration
    {
        public override void Up()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                SecuredEntityValidatorFactory factory = new SecuredEntityValidatorFactory();

                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, factory))
                {
                    IChampionRepository championRepository = new ChampionRepository(dataContext);

                    IChampionRecalculator championRecalculator = new ChampionRecalculator(dataContext, championRepository);
                    championRecalculator.RecalculateAllChampions();
                }
            }
        }
        
        public override void Down()
        {
        }
    }
}
