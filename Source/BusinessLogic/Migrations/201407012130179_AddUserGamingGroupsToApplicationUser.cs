namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserGamingGroupsToApplicationUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGamingGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        GamingGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GamingGroup", t => t.GamingGroupId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.GamingGroupId);

            Sql("INSERT INTO [dbo].[UserGamingGroup] (ApplicationUserId, GamingGroupId)"
                + "SELECT Id, (SELECT TOP 1 Id FROM GamingGroup) FROM AspNetUsers");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGamingGroup", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserGamingGroup", "GamingGroupId", "dbo.GamingGroup");
            DropIndex("dbo.UserGamingGroup", new[] { "GamingGroupId" });
            DropIndex("dbo.UserGamingGroup", new[] { "ApplicationUserId" });
            DropTable("dbo.UserGamingGroup");
        }
    }
}
