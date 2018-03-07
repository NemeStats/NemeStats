namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixInvalidWinnerTypes : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE PlayedGame SET WinnerType = 3 WHERE WinnerType = 1 AND NOT EXISTS(SELECT TOP 1 1 FROM PlayerGameResult WHERE PlayerGameResult.PlayedGameId = PlayedGame.Id AND PlayerGameResult.GameRank = 1)");
        }
        
        public override void Down()
        {
        }
    }
}
