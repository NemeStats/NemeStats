namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameDefinition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayedGame",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameDefinitionId = c.Int(nullable: false),
                        NumberOfPlayers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameDefinition", t => t.GameDefinitionId, cascadeDelete: true)
                .Index(t => t.GameDefinitionId);
            
            CreateTable(
                "dbo.PlayerGameResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayedGameId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        GameRank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayedGame", t => t.PlayedGameId, cascadeDelete: true)
                .ForeignKey("dbo.Player", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayedGameId)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.Player",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PlayedGame_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayedGame", t => t.PlayedGame_Id)
                .Index(t => t.PlayedGame_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "PlayedGame_Id", "dbo.PlayedGame");
            DropForeignKey("dbo.PlayerGameResult", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.PlayerGameResult", "PlayedGameId", "dbo.PlayedGame");
            DropForeignKey("dbo.PlayedGame", "GameDefinitionId", "dbo.GameDefinition");
            DropIndex("dbo.Player", new[] { "PlayedGame_Id" });
            DropIndex("dbo.PlayerGameResult", new[] { "PlayerId" });
            DropIndex("dbo.PlayerGameResult", new[] { "PlayedGameId" });
            DropIndex("dbo.PlayedGame", new[] { "GameDefinitionId" });
            DropTable("dbo.Player");
            DropTable("dbo.PlayerGameResult");
            DropTable("dbo.PlayedGame");
            DropTable("dbo.GameDefinition");
        }
    }
}
