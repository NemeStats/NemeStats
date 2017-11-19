namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GamingGroup_Active_Flag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroup", "Active", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GamingGroup", "Active");
        }
    }
}
