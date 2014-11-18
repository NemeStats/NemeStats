namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueConstraintOnUserGamingGroup : DbMigration
    {
        public override void Up()
        {
            this.Sql("DELETE FROM dbo.UserGamingGroup;");

            DropIndex("dbo.UserGamingGroup", new[] { "ApplicationUserId" });
            DropIndex("dbo.UserGamingGroup", new[] { "GamingGroupId" });
            CreateIndex("dbo.UserGamingGroup", new[] { "ApplicationUserId", "GamingGroupId" }, unique: true, name: "IX_USERID_AND_GAMING_GROUPID");
            
            this.Sql(@"INSERT INTO UserGamingGroup (ApplicationUserId, GamingGroupId) 
              (SELECT Id, CurrentGamingGroupid FROM AspNetUsers 
              WHERE NOT EXISTS (SELECT 1 FROM UserGamingGroup UGG2 WHERE UGG2.ApplicationUserId = AspNetUsers.Id 
              AND UGG2.GamingGroupId = AspNetUsers.CurrentGamingGroupId))");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserGamingGroup", "IX_USERID_AND_GAMING_GROUPID");
            CreateIndex("dbo.UserGamingGroup", "GamingGroupId");
            CreateIndex("dbo.UserGamingGroup", "ApplicationUserId");
        }
    }
}
