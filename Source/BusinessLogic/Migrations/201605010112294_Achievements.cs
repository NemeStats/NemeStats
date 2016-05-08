namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Achievements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achievement",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        FontAwesomeIcon = c.String(),
                        AchievementLevel1Threshold = c.Int(nullable: false),
                        AchievementLevel2Threshold = c.Int(nullable: false),
                        AchievementLevel3Threshold = c.Int(nullable: false),
                        PlayerCalculationSql = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayerAchievement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        AchievementId = c.Int(nullable: false),
                        AchievementLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Achievement", t => t.AchievementId, cascadeDelete: true)
                .ForeignKey("dbo.Player", t => t.PlayerId, cascadeDelete: true)
                .Index(t => new { t.PlayerId, t.AchievementId }, unique: true, name: "IX_PLAYERID_AND_ACHIEVEMENTID");

            Sql(@"INSERT INTO dbo.Achievement (Id, Name, Description, FontAwesomeIcon, AchievementLevel1Threshold, AchievementLevel2Threshold, AchievementLevel3Threshold, PlayerCalculationSql) 
            VALUES (1, 'Diversified', 'Played at least {0} different games.', 'fa-trophy', 5, 20, 100, 
            'SELECT COUNT(DISTINCT PlayedGame.GameDefinitionId) AS Value
              FROM[dbo].[PlayerGameResult]
              INNER JOIN[dbo].[PlayedGame] on PlayerGameResult.PlayedGameId = PlayedGame.Id
              WHERE PlayerGameResult.PlayerId = @playerId')");
        }
        
        public override void Down()
        {
            Sql("DELETE FROM Achievement");
            DropForeignKey("dbo.PlayerAchievement", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.PlayerAchievement", "AchievementId", "dbo.Achievement");
            DropIndex("dbo.PlayerAchievement", "IX_PLAYERID_AND_ACHIEVEMENTID");
            DropTable("dbo.PlayerAchievement");
            DropTable("dbo.Achievement");
        }
    }
}
