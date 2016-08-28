namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayedGameApplicationLinkage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayedGameApplicationLinkage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayedGameId = c.Int(nullable: false),
                        ApplicationName = c.String(nullable: false, maxLength: 255),
                        EntityId = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayedGame", t => t.PlayedGameId, cascadeDelete: true)
                .Index(t => new { t.PlayedGameId, t.ApplicationName, t.EntityId }, unique: true, name: "IX_PLAYEDGAMEID_APPLICATIONID_ENTITYID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayedGameApplicationLinkage", "PlayedGameId", "dbo.PlayedGame");
            DropIndex("dbo.PlayedGameApplicationLinkage", "IX_PLAYEDGAMEID_APPLICATIONID_ENTITYID");
            DropTable("dbo.PlayedGameApplicationLinkage");
        }
    }
}
