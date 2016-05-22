namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AchievementRelatedEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerAchievement", "RelatedEntities_PlainArray", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerAchievement", "RelatedEntities_PlainArray");
        }
    }
}
