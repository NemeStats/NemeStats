namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FixPreviousAndAddDateCreatedToPlayedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayedGame", "DatePlayed", c => c.DateTime(nullable: false));
            DropColumn("dbo.GameDefinition", "DatePlayed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameDefinition", "DatePlayed", c => c.DateTime(nullable: false));
            DropColumn("dbo.PlayedGame", "DatePlayed");
        }
    }
}
