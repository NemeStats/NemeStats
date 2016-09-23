namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BGGMechanics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardGameGeekGameMechanic",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardGameGeekGameMechanicId = c.Int(nullable: false),
                        MechanicName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BoardGameGeekGameToMechanic",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        BoardGameGeekGameMechanicId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.BoardGameGeekGameMechanicId })
                .ForeignKey("dbo.BoardGameGeekGameDefinition", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.BoardGameGeekGameMechanic", t => t.BoardGameGeekGameMechanicId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.BoardGameGeekGameMechanicId);           
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardGameGeekGameToMechanic", "BoardGameGeekGameDefinition", "dbo.BoardGameGeekGameDefinition");
            DropForeignKey("dbo.BoardGameGeekGameToMechanic", "BoardGameGeekGameMechanic", "dbo.BoardGameGeekGameMechanic");
            DropIndex("dbo.BoardGameGeekGameToMechanic", new[] { "BoardGameGeekGameDefinition_Id" });
            DropIndex("dbo.BoardGameGeekGameToMechanic", new[] { "BoardGameGeekGameMechanic_Id" });
            DropTable("dbo.BoardGameGeekGameToMechanic");
            DropTable("dbo.BoardGameGeekGameMechanic");
        }
    }
}
