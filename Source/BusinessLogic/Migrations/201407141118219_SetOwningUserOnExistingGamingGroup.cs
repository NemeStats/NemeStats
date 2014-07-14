namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetOwningUserOnExistingGamingGroup : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE GamingGroup SET OwningUserId = (SELECT TOP 1 Id FROM AspNetUsers)");
            DropForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GamingGroup", new[] { "OwningUserId" });
            AlterColumn("dbo.GamingGroup", "OwningUserId", c => c.String(maxLength: 128, nullable: false));
            CreateIndex("dbo.GamingGroup", "OwningUserId");
            AddForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            Sql("UPDATE GamingGroup SET OwningUserId = NULL");
            AlterColumn("dbo.GamingGroup", "OwningUserId", c => c.String(maxLength: 128, nullable: true));
        }
    }
}
