namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPublicFieldsToGamingGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroup", "PublicDescription", c => c.String());
            AddColumn("dbo.GamingGroup", "PublicGamingGroupWebsite", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamingGroup", "PublicGamingGroupWebsite");
            DropColumn("dbo.GamingGroup", "PublicDescription");
        }
    }
}
