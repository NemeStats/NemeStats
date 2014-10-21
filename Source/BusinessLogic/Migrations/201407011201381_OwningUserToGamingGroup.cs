namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class OwningUserToGamingGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroup", "OwningUserId", c => c.String(maxLength: 128, nullable: true));
            CreateIndex("dbo.GamingGroup", "OwningUserId");
            AddForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GamingGroup", new[] { "OwningUserId" });
            DropColumn("dbo.GamingGroup", "OwningUserId");
        }
    }
}
