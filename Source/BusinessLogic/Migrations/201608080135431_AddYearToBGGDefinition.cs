namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddYearToBGGDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekGameDefinition", "YearPublished", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "YearPublished");
        }
    }
}
