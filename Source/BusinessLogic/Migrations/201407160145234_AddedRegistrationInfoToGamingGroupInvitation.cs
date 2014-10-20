namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRegistrationInfoToGamingGroupInvitation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroupInvitation", "RegisteredUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.GamingGroupInvitation", "DateRegistered", c => c.DateTime());
            CreateIndex("dbo.GamingGroupInvitation", "RegisteredUserId");
            AddForeignKey("dbo.GamingGroupInvitation", "RegisteredUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroupInvitation", "RegisteredUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GamingGroupInvitation", new[] { "RegisteredUserId" });
            DropColumn("dbo.GamingGroupInvitation", "DateRegistered");
            DropColumn("dbo.GamingGroupInvitation", "RegisteredUserId");
        }
    }
}
