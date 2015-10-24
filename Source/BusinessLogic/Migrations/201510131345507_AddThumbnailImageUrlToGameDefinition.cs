namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddThumbnailImageUrlToGameDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameDefinition", "ThumbnailImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameDefinition", "ThumbnailImageUrl");
        }
    }
}
