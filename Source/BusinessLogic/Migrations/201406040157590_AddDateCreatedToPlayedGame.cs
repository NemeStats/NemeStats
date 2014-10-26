namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddDateCreatedToPlayedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameDefinition", "DatePlayed", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameDefinition", "DatePlayed");
        }
    }
}
