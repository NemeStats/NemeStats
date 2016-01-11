namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekUserDefinition_RemoveFKMapping : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            DropIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId");
            RenameColumn(table: "dbo.BoardGameGeekUserDefinition", name: "ApplicationUser_Id", newName: "ApplicationUserId");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            RenameColumn(table: "dbo.BoardGameGeekUserDefinition", name: "ApplicationUserId", newName: "ApplicationUser_Id");
            AddColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BoardGameGeekUserDefinition", "ApplicationUser_Id");
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
        }
    }
}
