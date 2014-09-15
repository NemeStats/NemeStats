namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerNameIsNoLongerNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Player", "IX_ID_AND_NAME");
            AlterColumn("dbo.Player", "Name", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.Player", new[] { "GamingGroupId", "Name" }, unique: true, name: "IX_ID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Player", "IX_ID_AND_NAME");
            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 255));
            CreateIndex("dbo.Player", new[] { "GamingGroupId", "Name" }, unique: true, name: "IX_ID_AND_NAME");
        }
    }
}
