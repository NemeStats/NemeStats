namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TotalPoints_ComputedProperty : DbMigration
    {
        public override void Up()
        {            
            Sql("ALTER TABLE dbo.PlayerGameResult ADD TotalPoints AS NemeStatsPointsAwarded + GameWeightBonusPoints + GameDurationBonusPoints");
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerGameResult", "TotalPoints");
        }
    }
}
