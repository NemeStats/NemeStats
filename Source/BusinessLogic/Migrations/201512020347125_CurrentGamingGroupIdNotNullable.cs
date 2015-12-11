namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrentGamingGroupIdNotNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "CurrentGamingGroupId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "CurrentGamingGroupId", c => c.Int());
        }
    }
}
