namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekUserDefinition_BGGUserIdOnApplicationUser : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            DropPrimaryKey("dbo.BoardGameGeekUserDefinition");
            AddColumn("dbo.AspNetUsers", "BoardGameGeekUserDefinitionId", c => c.String());
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.BoardGameGeekUserDefinition", "ApplicationUserId");
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            DropPrimaryKey("dbo.BoardGameGeekUserDefinition");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.AspNetUsers", "BoardGameGeekUserDefinitionId");
            AddPrimaryKey("dbo.BoardGameGeekUserDefinition", "Id");
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
        }
    }
}
