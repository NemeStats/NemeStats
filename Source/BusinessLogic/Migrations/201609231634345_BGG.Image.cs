namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BGGImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekGameDefinition", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "Image");
        }
    }
}
