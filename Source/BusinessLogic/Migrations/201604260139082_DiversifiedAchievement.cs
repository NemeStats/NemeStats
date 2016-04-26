namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiversifiedAchievement : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO dbo.Achievement (Id, Name, Description, FontAwesomeIcon, AchievementLevel1Threshold, AchievementLevel2Threshold, AchievementLevel3Threshold, PlayerCalculationSql) 
            VALUES (1, 'Diversified', 'Played at least {0} different games.', 'fa-trophy', 5, 20, 100, 
            'SELECT COUNT(DISTINCT PlayedGame.GameDefinitionId) AS Value
              FROM[dbo].[PlayerGameResult]
              INNER JOIN[dbo].[PlayedGame] on PlayerGameResult.PlayedGameId = PlayedGame.Id
              WHERE PlayerGameResult.PlayerId = @playerId')");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM Achievement WHERE Id = 1");
        }
    }
}
