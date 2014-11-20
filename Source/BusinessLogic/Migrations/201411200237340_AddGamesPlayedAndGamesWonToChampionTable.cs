namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGamesPlayedAndGamesWonToChampionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Champion", "NumberOfGames", c => c.Int(nullable: false));
            AddColumn("dbo.Champion", "NumberOfWins", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Champion", "NumberOfWins");
            DropColumn("dbo.Champion", "NumberOfGames");
        }
    }
}
