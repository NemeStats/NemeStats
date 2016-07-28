namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GamingGroupInvitation", "PlayerId", "dbo.Player");
            DropIndex("dbo.GamingGroupInvitation", new[] { "PlayerId" });
            AlterColumn("dbo.GamingGroupInvitation", "PlayerId", c => c.Int(nullable: false));
            CreateIndex("dbo.GamingGroupInvitation", "PlayerId");
            AddForeignKey("dbo.GamingGroupInvitation", "PlayerId", "dbo.Player", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroupInvitation", "PlayerId", "dbo.Player");
            DropIndex("dbo.GamingGroupInvitation", new[] { "PlayerId" });
            AlterColumn("dbo.GamingGroupInvitation", "PlayerId", c => c.Int());
            CreateIndex("dbo.GamingGroupInvitation", "PlayerId");
            AddForeignKey("dbo.GamingGroupInvitation", "PlayerId", "dbo.Player", "Id");
        }
    }
}
