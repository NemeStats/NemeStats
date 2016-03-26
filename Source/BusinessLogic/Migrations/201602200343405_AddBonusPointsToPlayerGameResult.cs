namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBonusPointsToPlayerGameResult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerGameResult", "GameWeightBonusPoints", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.PlayerGameResult", "GameDurationBonusPoints", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerGameResult", "GameDurationBonusPoints");
            DropColumn("dbo.PlayerGameResult", "GameWeightBonusPoints");
        }
    }
}
