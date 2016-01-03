namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBGGUserDefinition_ApplicationUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekUserDefinition", "ApplicationUserId");
        }
    }
}
