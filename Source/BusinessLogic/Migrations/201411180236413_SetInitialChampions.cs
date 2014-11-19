using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetInitialChampions : DbMigration
    {
        public override void Up()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
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
