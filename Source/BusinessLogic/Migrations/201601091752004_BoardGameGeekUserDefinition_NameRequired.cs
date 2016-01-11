namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekUserDefinition_NameRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            AddColumn("dbo.BoardGameGeekUserDefinition", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
            CreateIndex("dbo.BoardGameGeekUserDefinition", "User_Id");
            AddForeignKey("dbo.BoardGameGeekUserDefinition", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardGameGeekUserDefinition", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BoardGameGeekUserDefinition", new[] { "User_Id" });
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(maxLength: 50));
            DropColumn("dbo.BoardGameGeekUserDefinition", "User_Id");
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
            AddForeignKey("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
