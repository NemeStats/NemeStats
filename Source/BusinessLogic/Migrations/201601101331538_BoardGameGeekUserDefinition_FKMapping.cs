namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoardGameGeekUserDefinition_FKMapping : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.BoardGameGeekUserDefinition", name: "User_Id", newName: "ApplicationUser_Id");
            RenameIndex(table: "dbo.BoardGameGeekUserDefinition", name: "IX_User_Id", newName: "IX_ApplicationUser_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.BoardGameGeekUserDefinition", name: "IX_ApplicationUser_Id", newName: "IX_User_Id");
            RenameColumn(table: "dbo.BoardGameGeekUserDefinition", name: "ApplicationUser_Id", newName: "User_Id");
        }
    }
}
