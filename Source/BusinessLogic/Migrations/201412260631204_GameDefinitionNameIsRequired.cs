namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameDefinitionNameIsRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GameDefinition", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GameDefinition", "Name", c => c.String());
        }
    }
}
