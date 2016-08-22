namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSyncableFieldsToPlayedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayedGame", "ExternalSourceApplicationName", c => c.String());
            AddColumn("dbo.PlayedGame", "ExternalSourceEntityId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayedGame", "ExternalSourceEntityId");
            DropColumn("dbo.PlayedGame", "ExternalSourceApplicationName");
        }
    }
}
