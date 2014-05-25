namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LowerCharacterLimitOfPlayerNameAndAddUniqueIndex : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 500));

            CreateIndex(table: "Player",
                column: "Name",
                unique: true,
                name: "UniqueNameIndex");
        }

        public override void Down()
        {
            AlterColumn("dbo.Player", "Name", c => c.String());

            DropIndex(table: "Player",
                name: "UniqueNameIndex");
        }
    }
}
