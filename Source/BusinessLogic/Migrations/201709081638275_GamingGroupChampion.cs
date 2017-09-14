namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GamingGroupChampion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroup", "GamingGroupChampionPlayerId", c => c.Int());
            CreateIndex("dbo.GamingGroup", "GamingGroupChampionPlayerId");
            AddForeignKey("dbo.GamingGroup", "GamingGroupChampionPlayerId", "dbo.Player", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroup", "GamingGroupChampionPlayerId", "dbo.Player");
            DropIndex("dbo.GamingGroup", new[] { "GamingGroupChampionPlayerId" });
            DropColumn("dbo.GamingGroup", "GamingGroupChampionPlayerId");
        }
    }
}
