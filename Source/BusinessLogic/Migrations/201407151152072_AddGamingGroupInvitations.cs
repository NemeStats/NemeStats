namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddGamingGroupInvitations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GamingGroupInvitation",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GamingGroupId = c.Int(nullable: false),
                        InviteeEmail = c.String(maxLength: 255),
                        InvitingUserId = c.String(maxLength: 128),
                        DateSent = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitingUserId)
                .ForeignKey("dbo.GamingGroup", t => t.GamingGroupId, cascadeDelete: true)
                .Index(t => t.GamingGroupId)
                .Index(t => t.InvitingUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroupInvitation", "GamingGroupId", "dbo.GamingGroup");
            DropForeignKey("dbo.GamingGroupInvitation", "InvitingUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GamingGroupInvitation", new[] { "InvitingUserId" });
            DropIndex("dbo.GamingGroupInvitation", new[] { "GamingGroupId" });
            DropTable("dbo.GamingGroupInvitation");
        }
    }
}
