namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekGameDefinitionIdentityInsert : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition");
            DropPrimaryKey("dbo.BoardGameGeekGameDefinition");
            AlterColumn("dbo.BoardGameGeekGameDefinition", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.BoardGameGeekGameDefinition", "Id");
            AddForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition");
            DropPrimaryKey("dbo.BoardGameGeekGameDefinition");
            AlterColumn("dbo.BoardGameGeekGameDefinition", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.BoardGameGeekGameDefinition", "Id");
            AddForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition", "Id");
        }
    }
}
