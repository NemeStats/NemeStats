namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueConstraintOnPlayerIdAndPlayedGameIdInPlayerGameResult : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlayerGameResult", new[] { "PlayedGameId" });
            DropIndex("dbo.PlayerGameResult", new[] { "PlayerId" });
            CreateIndex("dbo.PlayerGameResult", new[] { "PlayedGameId", "PlayerId" }, unique: true, name: "IX_PlayerId_and_PlayedGameId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlayerGameResult", "IX_PlayerId_and_PlayedGameId");
            CreateIndex("dbo.PlayerGameResult", "PlayerId");
            CreateIndex("dbo.PlayerGameResult", "PlayedGameId");
        }
    }
}
