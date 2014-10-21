namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakeCurrentGamingGroupNullableOnApplicationUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "CurrentGamingGroupId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "CurrentGamingGroupId", c => c.Int(nullable: false));
        }
    }
}
