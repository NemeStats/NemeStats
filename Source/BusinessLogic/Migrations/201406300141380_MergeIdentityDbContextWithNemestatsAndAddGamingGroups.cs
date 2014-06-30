namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeIdentityDbContextWithNemestatsAndAddGamingGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GamingGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OwningUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwningUserId)
                .Index(t => t.OwningUserId);

            Sql("INSERT INTO GamingGroup (Name, OwningUserId) VALUES ('RIDGID Board Gamers', (SELECT TOP 1 Id FROM AspNetUsers)) ");

            AddColumn("dbo.GameDefinition", "GamingGroupId", c => c.Int(nullable: true));
            Sql("UPDATE dbo.GameDefinition SET GamingGroupId = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.GameDefinition", "GamingGroupId", c => c.Int(nullable: false));

            AddColumn("dbo.PlayedGame", "GamingGroupId", c => c.Int(nullable: true));
            Sql("UPDATE dbo.PlayedGame SET GamingGroupId = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.PlayedGame", "GamingGroupId", c => c.Int(nullable: false));

            AddColumn("dbo.PlayerGameResult", "GamingGroupId", c => c.Int(nullable: true));
            Sql("UPDATE dbo.PlayerGameResult SET GamingGroupId = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.PlayerGameResult", "GamingGroupId", c => c.Int(nullable: false));

            AddColumn("dbo.Player", "GamingGroupId", c => c.Int(nullable: true));
            Sql("UPDATE dbo.Player SET GamingGroupId = (SELECT TOP 1 Id FROM GamingGroup)");
            AlterColumn("dbo.Player", "GamingGroupId", c => c.Int(nullable: false));

            CreateIndex("dbo.GameDefinition", "GamingGroupId");
            CreateIndex("dbo.PlayedGame", "GamingGroupId");
            CreateIndex("dbo.PlayerGameResult", "GamingGroupId");
            CreateIndex("dbo.Player", "GamingGroupId");
            AddForeignKey("dbo.GameDefinition", "GamingGroupId", "dbo.GamingGroup", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PlayedGame", "GamingGroupId", "dbo.GamingGroup", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PlayerGameResult", "GamingGroupId", "dbo.GamingGroup", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Player", "GamingGroupId", "dbo.GamingGroup", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "GamingGroupId", "dbo.GamingGroup");
            DropForeignKey("dbo.PlayerGameResult", "GamingGroupId", "dbo.GamingGroup");
            DropForeignKey("dbo.PlayedGame", "GamingGroupId", "dbo.GamingGroup");
            DropForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.IdentityUser");
            DropForeignKey("dbo.GameDefinition", "GamingGroupId", "dbo.GamingGroup");
            DropIndex("dbo.Player", new[] { "GamingGroupId" });
            DropIndex("dbo.PlayerGameResult", new[] { "GamingGroupId" });
            DropIndex("dbo.PlayedGame", new[] { "GamingGroupId" });
            DropIndex("dbo.GamingGroup", new[] { "OwningUserId" });
            DropIndex("dbo.GameDefinition", new[] { "GamingGroupId" });
            DropColumn("dbo.Player", "GamingGroupId");
            DropColumn("dbo.PlayerGameResult", "GamingGroupId");
            DropColumn("dbo.PlayedGame", "GamingGroupId");
            DropColumn("dbo.GameDefinition", "GamingGroupId");
            DropTable("dbo.GamingGroup");
        }
    }
}
