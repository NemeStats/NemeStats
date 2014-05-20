namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Player", "PlayedGame_Id", "dbo.PlayedGame");
            DropIndex("dbo.Player", new[] { "PlayedGame_Id" });
            AddColumn("dbo.Player", "Active", c => c.Boolean(nullable: false));
            DropColumn("dbo.Player", "PlayedGame_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Player", "PlayedGame_Id", c => c.Int());
            DropColumn("dbo.Player", "Active");
            CreateIndex("dbo.Player", "PlayedGame_Id");
            AddForeignKey("dbo.Player", "PlayedGame_Id", "dbo.PlayedGame", "Id");
        }
    }
}
