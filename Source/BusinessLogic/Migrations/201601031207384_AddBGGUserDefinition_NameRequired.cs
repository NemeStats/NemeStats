namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBGGUserDefinition_NameRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BoardGameGeekUserDefinition", new[] { "Name" });
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BoardGameGeekUserDefinition", "Name", c => c.String());
            CreateIndex("dbo.BoardGameGeekUserDefinition", "Name", unique: true);
        }
    }
}
