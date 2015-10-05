namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChangeGordonPointsToNemeStatsPoints : DbMigration
    {
        public override void Up()
        {
            this.RenameColumn("dbo.PlayerGameResult", "GordonPoints", "NemeStatsPointsAwarded");
        }
        
        public override void Down()
        {
            this.RenameColumn("dbo.PlayerGameResult", "NemeStatsPointsAwarded", "GordonPoints");
        }
    }
}
