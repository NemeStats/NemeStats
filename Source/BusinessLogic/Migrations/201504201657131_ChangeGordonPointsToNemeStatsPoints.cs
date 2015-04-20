namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeGordonPointsToNemeStatsPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerGameResult", "NemeStatsPointsAwarded", c => c.Int(nullable: false));
            DropColumn("dbo.PlayerGameResult", "GordonPoints");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerGameResult", "GordonPoints", c => c.Int(nullable: false));
            DropColumn("dbo.PlayerGameResult", "NemeStatsPointsAwarded");
        }
    }
}
