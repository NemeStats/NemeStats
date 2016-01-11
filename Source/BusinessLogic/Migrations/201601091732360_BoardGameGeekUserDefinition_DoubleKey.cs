namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekUserDefinition_DoubleKey : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_ID_AND_NAME");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "ApplicationUserId", "Name" }, unique: true, name: "IX_USERID_AND_NAME");
            AddForeignKey("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            AlterColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String());
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(maxLength: 255));
            CreateIndex("dbo.BoardGameGeekUserDefinition", new[] { "Name", "ApplicationUserId" }, unique: true, name: "IX_ID_AND_NAME");
        }
    }
}
