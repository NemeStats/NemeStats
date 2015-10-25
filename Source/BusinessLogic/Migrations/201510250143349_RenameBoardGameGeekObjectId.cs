namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameBoardGameGeekObjectId : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardGameGeekGameDefinition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: false),
                        Name = c.String(),
                        Description = c.String(),
                        Thumbnail = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            RenameColumn("dbo.GameDefinition", "BoardGameGeekObjectId", "BoardGameGeekGameDefinitionId");
            CreateIndex("dbo.GameDefinition", "BoardGameGeekGameDefinitionId");
            //AddForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition", "Id");
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "dbo.BoardGameGeekGameDefinition");
            DropIndex("dbo.GameDefinition", new[] { "BoardGameGeekGameDefinitionId" });
            RenameColumn("dbo.GameDefinition", "BoardGameGeekGameDefinitionId", "BoardGameGeekObjectId");
            DropTable("dbo.BoardGameGeekGameDefinition");
        }
    }
}
