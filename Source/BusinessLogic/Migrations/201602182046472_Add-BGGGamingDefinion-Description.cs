namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBGGGamingDefinionDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekGameDefinition", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "Description");
        }
    }
}
