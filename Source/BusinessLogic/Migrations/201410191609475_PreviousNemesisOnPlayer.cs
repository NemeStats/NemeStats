namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PreviousNemesisOnPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "PreviousNemesisId", c => c.Int());
            CreateIndex("dbo.Player", "PreviousNemesisId");
            AddForeignKey("dbo.Player", "PreviousNemesisId", "dbo.Nemesis", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "PreviousNemesisId", "dbo.Nemesis");
            DropIndex("dbo.Player", new[] { "PreviousNemesisId" });
            DropColumn("dbo.Player", "PreviousNemesisId");
        }
    }
}
