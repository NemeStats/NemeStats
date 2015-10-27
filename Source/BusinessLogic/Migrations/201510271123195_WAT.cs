namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WAT : DbMigration
    {
        //was in a bad state and unfortunately the only way to fix it was to add and run a completely pointless migration
        public override void Up()
        {
            //DropColumn("dbo.BoardGameGeekGameDefinition", "Description");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.BoardGameGeekGameDefinition", "Description", c => c.String());
        }
    }
}
