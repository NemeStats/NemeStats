namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueIndexToPlayerNameAndGamingGroupIp : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Player", new[] { "GamingGroupId" });
            CreateIndex("dbo.Player", new[] { "GamingGroupId", "Name" }, unique: true, name: "IX_ID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Player", "IX_ID_AND_NAME");
            CreateIndex("dbo.Player", "GamingGroupId");
        }
    }
}
