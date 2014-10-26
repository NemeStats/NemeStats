namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LowerCharacterLimitOfPlayerNameTo255 : DbMigration
    {
        public override void Up()
        {
            DropIndex(table: "Player",
                name: "UniqueNameIndex");

            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 255));

            CreateIndex(table: "Player",
                column: "Name",
                unique: true,
                name: "UniqueNameIndex");
        }
        
        public override void Down()
        {
            DropIndex(table: "Player",
                name: "UniqueNameIndex");

            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 500));

            CreateIndex(table: "Player",
                column: "Name",
                unique: true,
                name: "UniqueNameIndex");
        }
    }
}
