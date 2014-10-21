namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CurrentGamingGroupIdIsNotNullable : DbMigration
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
