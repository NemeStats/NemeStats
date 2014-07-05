namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCurrentGamingGroupIdToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CurrentGamingGroupId", c => c.Int(nullable: true));
            Sql("UPDATE dbo.AspNetUsers SET CurrentGamingGroupId = (SELECT TOP 1 GamingGroupId FROM UserGamingGroup WHERE UserGamingGroup.ApplicationUserId = AspNetUsers.Id)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CurrentGamingGroupId");
        }
    }
}
