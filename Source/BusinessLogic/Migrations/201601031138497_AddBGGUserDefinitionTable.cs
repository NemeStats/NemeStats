namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBGGUserDefinitionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardGameGeekUserDefinition",
                c => new
                {
                    Id = c.Int(nullable: false),
                    Avatar = c.String(),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);

        }
        
        public override void Down()
        {
            DropTable("dbo.BoardGameGeekUserDefinition");
        }
    }
}
