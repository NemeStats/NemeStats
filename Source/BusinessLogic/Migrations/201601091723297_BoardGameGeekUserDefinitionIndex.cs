namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekUserDefinitionIndex : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.BoardGameGeekUserDefinition");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(maxLength: 255));
            AddPrimaryKey("dbo.BoardGameGeekUserDefinition", "Id");
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "Name", "ApplicationUserId" }, unique: true, name: "IX_ID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_ID_AND_NAME");
            DropPrimaryKey("dbo.BoardGameGeekUserDefinition");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.BoardGameGeekUserDefinition", "Id");
        }
    }
}
