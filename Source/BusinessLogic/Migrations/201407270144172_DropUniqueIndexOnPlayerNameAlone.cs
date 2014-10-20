namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropUniqueIndexOnPlayerNameAlone : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Player", "UniqueNameIndex");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Player", new[] { "Name" }, unique: true, name: "UniqueNameIndex");
        }
    }
}
