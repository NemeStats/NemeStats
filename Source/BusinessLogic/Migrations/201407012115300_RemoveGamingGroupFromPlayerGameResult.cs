namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGamingGroupFromPlayerGameResult : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerGameResult", "GamingGroupId", "dbo.GamingGroup");
            DropIndex("dbo.PlayerGameResult", new[] { "GamingGroupId" });
            DropColumn("dbo.PlayerGameResult", "GamingGroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerGameResult", "GamingGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.PlayerGameResult", "GamingGroupId");
            AddForeignKey("dbo.PlayerGameResult", "GamingGroupId", "dbo.GamingGroup", "Id", cascadeDelete: true);
        }
    }
}
