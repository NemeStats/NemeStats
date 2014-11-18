namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateChampionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Champion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameDefinitionId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        WinPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameDefinition", t => t.GameDefinitionId, cascadeDelete: false)
                .ForeignKey("dbo.Player", t => t.PlayerId, cascadeDelete: false)
                .Index(t => t.GameDefinitionId)
                .Index(t => t.PlayerId);
            
            AddColumn("dbo.GameDefinition", "ChampionId", c => c.Int());
            AddColumn("dbo.GameDefinition", "PreviousChampionId", c => c.Int());
            CreateIndex("dbo.GameDefinition", "ChampionId");
            CreateIndex("dbo.GameDefinition", "PreviousChampionId");
            AddForeignKey("dbo.GameDefinition", "ChampionId", "dbo.Champion", "Id");
            AddForeignKey("dbo.GameDefinition", "PreviousChampionId", "dbo.Champion", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Champion", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.Champion", "GameDefinitionId", "dbo.GameDefinition");
            DropForeignKey("dbo.GameDefinition", "PreviousChampionId", "dbo.Champion");
            DropForeignKey("dbo.GameDefinition", "ChampionId", "dbo.Champion");
            DropIndex("dbo.GameDefinition", new[] { "PreviousChampionId" });
            DropIndex("dbo.GameDefinition", new[] { "ChampionId" });
            DropIndex("dbo.Champion", new[] { "PlayerId" });
            DropIndex("dbo.Champion", new[] { "GameDefinitionId" });
            DropColumn("dbo.GameDefinition", "PreviousChampionId");
            DropColumn("dbo.GameDefinition", "ChampionId");
            DropTable("dbo.Champion");
        }
    }
}
