namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayedGameDateUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayedGame", "DateUpdated", c => c.DateTime(nullable: true));

            Sql("UPDATE PlayedGame SET DateUpdated = DateCreated");

            AlterColumn("dbo.PlayedGame", "DateUpdated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayedGame", "DateUpdated");
        }
    }
}
