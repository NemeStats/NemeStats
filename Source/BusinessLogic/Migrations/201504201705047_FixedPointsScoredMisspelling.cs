namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FixedPointsScoredMisspelling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerGameResult", "PointsScored", c => c.Int());
            DropColumn("dbo.PlayerGameResult", "PointScored");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerGameResult", "PointScored", c => c.Int());
            DropColumn("dbo.PlayerGameResult", "PointsScored");
        }
    }
}
