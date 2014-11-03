namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBoardGameGeekObjectId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameDefinition", "BoardGameGeekObjecdtId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GameDefinition", "BoardGameGeekObjecdtId");
        }
    }
}
