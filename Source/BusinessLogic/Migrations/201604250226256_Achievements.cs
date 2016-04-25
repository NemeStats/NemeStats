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
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayerAchievement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        Notes = c.String(),
                        PlayedGameId = c.Int(),
                        PlayerId = c.Int(nullable: false),
                        AchievementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Achievement", t => t.AchievementId, cascadeDelete: true)
                .ForeignKey("dbo.Player", t => t.PlayerId, cascadeDelete: true)
                .ForeignKey("dbo.PlayedGame", t => t.PlayedGameId)
                .Index(t => t.PlayedGameId)
                .Index(t => new { t.PlayerId, t.AchievementId }, unique: true, name: "IX_PLAYERID_AND_ACHIEVEMENTID");

            //--single example Achievement
            Sql("INSERT INTO dbo.Achievement (Id, Name, Description, FontAwesomeIcon) VALUES (1, 'Diversified', 'Played at least 5 different games.', 'fa-trophy')");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerAchievement", "PlayedGameId", "dbo.PlayedGame");
            DropForeignKey("dbo.PlayerAchievement", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.PlayerAchievement", "AchievementId", "dbo.Achievement");
            DropIndex("dbo.PlayerAchievement", "IX_PLAYERID_AND_ACHIEVEMENTID");
            DropIndex("dbo.PlayerAchievement", new[] { "PlayedGameId" });
            DropTable("dbo.PlayerAchievement");
            DropTable("dbo.Achievement");
        }
    }
}
