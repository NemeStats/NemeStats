using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddWinnerTypeToPlayedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayedGame", "WinnerType", c => c.Int(nullable: false, defaultValue: (int)WinnerTypes.PlayerWin));
            Sql(@"UPDATE dbo.PlayedGame SET WinnerType = 2 
                WHERE 1 = ALL 
	            (SELECT GameRank FROM dbo.PlayerGameResult 
	            WHERE PlayerGameResult.PlayedGameId = PlayedGame.Id);");

            Sql(@"UPDATE dbo.PlayedGame SET WinnerType = 3 
                WHERE 1 < ALL 
	            (SELECT GameRank FROM dbo.PlayerGameResult 
	            WHERE PlayerGameResult.PlayedGameId = PlayedGame.Id);");
        }

        public override void Down()
        {
            DropColumn("dbo.PlayedGame", "WinnerType");
        }
    }
}
