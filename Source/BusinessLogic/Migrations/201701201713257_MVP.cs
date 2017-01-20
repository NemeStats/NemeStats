namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_MVP_Entity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MVP",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerGameResultId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        PointsScored = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DatePlayed = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayerGameResult", t => t.PlayerGameResultId, cascadeDelete: true)
                .Index(t => t.PlayerGameResultId);
            
            AddColumn("dbo.GameDefinition", "MVPId", c => c.Int());
            AddColumn("dbo.GameDefinition", "PreviousMVPId", c => c.Int());
            CreateIndex("dbo.GameDefinition", "MVPId");
            CreateIndex("dbo.GameDefinition", "PreviousMVPId");
            AddForeignKey("dbo.GameDefinition", "MVPId", "dbo.MVP", "Id");
            AddForeignKey("dbo.GameDefinition", "PreviousMVPId", "dbo.MVP", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameDefinition", "PreviousMVPId", "dbo.MVP");
            DropForeignKey("dbo.GameDefinition", "MVPId", "dbo.MVP");
            DropForeignKey("dbo.MVP", "PlayedGameResultId", "dbo.PlayerGameResult");
            DropIndex("dbo.MVP", new[] { "PlayedGameResultId" });
            DropIndex("dbo.GameDefinition", new[] { "PreviousMVPId" });
            DropIndex("dbo.GameDefinition", new[] { "MVPId" });
            DropColumn("dbo.GameDefinition", "PreviousMVPId");
            DropColumn("dbo.GameDefinition", "MVPId");
            DropTable("dbo.MVP");
        }
    }
}
