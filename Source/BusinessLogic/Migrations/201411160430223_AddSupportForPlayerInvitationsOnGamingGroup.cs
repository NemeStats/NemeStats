namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupportForPlayerInvitationsOnGamingGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroupInvitation", "PlayerId", c => c.Int());
            CreateIndex("dbo.GamingGroupInvitation", "PlayerId");
            AddForeignKey("dbo.GamingGroupInvitation", "PlayerId", "dbo.Player", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroupInvitation", "PlayerId", "dbo.Player");
            DropIndex("dbo.GamingGroupInvitation", new[] { "PlayerId" });
            DropColumn("dbo.GamingGroupInvitation", "PlayerId");
        }
    }
}
