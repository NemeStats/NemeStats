using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Points;

namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAllPointsViaNewFormula : DbMigration
    {
        public override void Up()
        {
            using (IDataContext dataContext = new NemeStatsDataContext())
            {
                new GlobalPointsRecalculator().RecalculateAllPoints(dataContext);
            }
        }
        
        public override void Down()
        {
        }
    }
}
