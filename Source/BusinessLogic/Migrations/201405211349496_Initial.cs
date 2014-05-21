namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Player", "PlayedGame_Id", "dbo.PlayedGame");
            DropIndex("dbo.Player", new[] { "PlayedGame_Id" });
            AddColumn("dbo.PlayerGameResult", "GameDefinition_Id", c => c.Int());
            AddColumn("dbo.Player", "Active", c => c.Boolean(nullable: false));
            CreateIndex("dbo.PlayerGameResult", "GameDefinition_Id");
            AddForeignKey("dbo.PlayerGameResult", "GameDefinition_Id", "dbo.GameDefinition", "Id");
            DropColumn("dbo.Player", "PlayedGame_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Player", "PlayedGame_Id", c => c.Int());
            DropForeignKey("dbo.PlayerGameResult", "GameDefinition_Id", "dbo.GameDefinition");
            DropIndex("dbo.PlayerGameResult", new[] { "GameDefinition_Id" });
            DropColumn("dbo.Player", "Active");
            DropColumn("dbo.PlayerGameResult", "GameDefinition_Id");
            CreateIndex("dbo.Player", "PlayedGame_Id");
            AddForeignKey("dbo.Player", "PlayedGame_Id", "dbo.PlayedGame", "Id");
        }
    }
}
