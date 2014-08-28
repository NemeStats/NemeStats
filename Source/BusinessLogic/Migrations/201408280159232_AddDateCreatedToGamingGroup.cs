namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateCreatedToGamingGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroup", "DateCreated", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamingGroup", "DateCreated");
        }
    }
}
