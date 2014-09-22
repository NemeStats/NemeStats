namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateNemesisTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nemesis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinionPlayerId = c.Int(nullable: false),
                        NemesisPlayerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false, defaultValue: DateTime.UtcNow),
                        NumberOfGamesLost = c.Int(nullable: false),
                        LossPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Player", t => t.MinionPlayerId, cascadeDelete: false)
                .ForeignKey("dbo.Player", t => t.NemesisPlayerId, cascadeDelete: false)
                .Index(t => t.MinionPlayerId)
                .Index(t => t.NemesisPlayerId);
            
            AddColumn("dbo.Player", "NemesisId", c => c.Int());
            CreateIndex("dbo.Player", "NemesisId");
            AddForeignKey("dbo.Player", "NemesisId", "dbo.Nemesis", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "NemesisId", "dbo.Nemesis");
            DropForeignKey("dbo.Nemesis", "NemesisPlayerId", "dbo.Player");
            DropForeignKey("dbo.Nemesis", "MinionPlayerId", "dbo.Player");
            DropIndex("dbo.Nemesis", new[] { "NemesisPlayerId" });
            DropIndex("dbo.Nemesis", new[] { "MinionPlayerId" });
            DropIndex("dbo.Player", new[] { "NemesisId" });
            DropColumn("dbo.Player", "NemesisId");
            DropTable("dbo.Nemesis");
        }
    }
}
