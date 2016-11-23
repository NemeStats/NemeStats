namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class BGGGameDefinition_Dates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekGameDefinition", "DateCreated", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
            AddColumn("dbo.BoardGameGeekGameDefinition", "DateUpdated", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
        }

        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "DateUpdated");
            DropColumn("dbo.BoardGameGeekGameDefinition", "DateCreated");
        }
    }
}
