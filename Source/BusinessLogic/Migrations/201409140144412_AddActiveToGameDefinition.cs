namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveToGameDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameDefinition", "Active", c => c.Boolean(nullable: false, defaultValue: true));
            Sql("UPDATE dbo.GameDefinition SET Active = 1;");

        }
        
        public override void Down()
        {
            DropColumn("dbo.GameDefinition", "Active");
        }
    }
}
