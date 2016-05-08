namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementBusinessLogic : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerAchievement", "AchievementId", "dbo.Achievement");
            AddColumn("dbo.PlayerAchievement", "LastUpdatedDate", c => c.DateTime(nullable: false));
            DropTable("dbo.Achievement");
        }
        
        public override void Down()
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
            
            DropColumn("dbo.PlayerAchievement", "LastUpdatedDate");
            AddForeignKey("dbo.PlayerAchievement", "AchievementId", "dbo.Achievement", "Id", cascadeDelete: true);
        }
    }
}
