namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAT2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition");
            DropIndex("dbo.GameDefinition", new[] { "BoardGameGeekGameDefinitionId" });
            AlterColumn("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", c => c.Int());
            CreateIndex("dbo.GameDefinition", "BoardGameGeekGameDefinitionId");
            AddForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition");
            DropIndex("dbo.GameDefinition", new[] { "BoardGameGeekGameDefinitionId" });
            AlterColumn("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", c => c.Int(nullable: false));
            CreateIndex("dbo.GameDefinition", "BoardGameGeekGameDefinitionId");
            AddForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition", "Id", cascadeDelete: true);
        }
    }
}
