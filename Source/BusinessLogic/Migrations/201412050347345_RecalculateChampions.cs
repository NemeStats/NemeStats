using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecalculateChampions : DbMigration
    {
        public override void Up()
        {
            //since the model has changed, since this migration was originally written, this is no longer valid.
            //using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            //{
            //    using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            //    {
            //        IChampionRepository championRepository = new ChampionRepository(dataContext);

            //        IChampionRecalculator championRecalculator = new ChampionRecalculator(dataContext, championRepository);
            //        championRecalculator.RecalculateAllChampions();
            //    }
            //}
        }
        
        public override void Down()
        {
        }
    }
}
