namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakePointsScoredDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PlayerGameResult", "PointsScored", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlayerGameResult", "PointsScored", c => c.Int());
        }
    }
}
