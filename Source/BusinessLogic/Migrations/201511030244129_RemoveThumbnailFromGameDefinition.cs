namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveThumbnailFromGameDefinition : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.GameDefinition", "ThumbnailImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameDefinition", "ThumbnailImageUrl", c => c.String());
        }
    }
}
