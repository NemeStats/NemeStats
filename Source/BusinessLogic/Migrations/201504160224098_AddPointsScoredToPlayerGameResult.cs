namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPointsScoredToPlayerGameResult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerGameResult", "PointScored", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerGameResult", "PointScored");
        }
    }
}
