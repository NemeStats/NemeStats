namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayerUserRelationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Player", "ApplicationUserId");
            AddForeignKey("dbo.Player", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Player", new[] { "ApplicationUserId" });
            DropColumn("dbo.Player", "ApplicationUserId");
        }
    }
}
