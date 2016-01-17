namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekDefinitionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardGameGeekUserDefinition",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false),
                        Avatar = c.String(),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ApplicationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => new { t.ApplicationUserId, t.Name }, unique: true, name: "IX_USERID_AND_NAME");
            
            AddColumn("dbo.AspNetUsers", "BoardGameGeekUserDefinitionId", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.BoardGameGeekUserDefinition", "IX_USERID_AND_NAME");
            DropColumn("dbo.AspNetUsers", "BoardGameGeekUserDefinitionId");
            DropTable("dbo.BoardGameGeekUserDefinition");
        }
    }
}
