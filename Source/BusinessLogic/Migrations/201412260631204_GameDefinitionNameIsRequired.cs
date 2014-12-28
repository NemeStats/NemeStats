namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameDefinitionNameIsRequired : DbMigration
    {
        public override void Up()
        {
            this.Sql("UPDATE GameDefinition SET Name = 'Game Name Not Set' WHERE Name IS NULL");
            AlterColumn("dbo.GameDefinition", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GameDefinition", "Name", c => c.String());
        }
    }
}
