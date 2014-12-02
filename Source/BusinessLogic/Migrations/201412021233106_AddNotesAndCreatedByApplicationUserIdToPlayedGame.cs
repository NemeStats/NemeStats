namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotesAndCreatedByApplicationUserIdToPlayedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayedGame", "CreatedByApplicationUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.PlayedGame", "Notes", c => c.String());
            CreateIndex("dbo.PlayedGame", "CreatedByApplicationUserId");
            AddForeignKey("dbo.PlayedGame", "CreatedByApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayedGame", "CreatedByApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.PlayedGame", new[] { "CreatedByApplicationUserId" });
            DropColumn("dbo.PlayedGame", "Notes");
            DropColumn("dbo.PlayedGame", "CreatedByApplicationUserId");
        }
    }
}
