namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BggCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardGameGeekGameCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardGameGeekGameCategoryId = c.Int(nullable: false),
                        CategoryName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BoardGameGeekGameToCategory",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        BoardGameGeekGameCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.BoardGameGeekGameCategoryId })
                .ForeignKey("dbo.BoardGameGeekGameDefinition", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.BoardGameGeekGameCategory", t => t.BoardGameGeekGameCategoryId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.BoardGameGeekGameCategoryId);
            
            AddColumn("dbo.BoardGameGeekGameDefinition", "IsExpansion", c => c.Boolean(nullable: false));
            AddColumn("dbo.BoardGameGeekGameDefinition", "Rank", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardGameGeekGameToCategory", "BoardGameGeekGameCategoryId", "dbo.BoardGameGeekGameCategory");
            DropForeignKey("dbo.BoardGameGeekGameToCategory", "Id", "dbo.BoardGameGeekGameDefinition");
            DropIndex("dbo.BoardGameGeekGameToCategory", new[] { "BoardGameGeekGameCategoryId" });
            DropIndex("dbo.BoardGameGeekGameToCategory", new[] { "Id" });
            DropColumn("dbo.BoardGameGeekGameDefinition", "Rank");
            DropColumn("dbo.BoardGameGeekGameDefinition", "IsExpansion");
            DropTable("dbo.BoardGameGeekGameToCategory");
            DropTable("dbo.BoardGameGeekGameCategory");
        }
    }
}
