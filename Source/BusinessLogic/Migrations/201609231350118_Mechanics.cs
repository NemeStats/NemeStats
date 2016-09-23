namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingMechanicsMigration : DbMigration
    {
        public override void Up()
        {
            //RenameTable(name: "dbo.BoardGameGeekGameMechanicBoardGameGeekGameDefinition", newName: "BoardGameGeekGameToMechanic");
            //RenameColumn(table: "dbo.BoardGameGeekGameToMechanic", name: "BoardGameGeekGameMechanic_Id", newName: "BoardGameGeekGameMechanicId");
            //RenameColumn(table: "dbo.BoardGameGeekGameToMechanic", name: "BoardGameGeekGameDefinition_Id", newName: "Id");
            //RenameIndex(table: "dbo.BoardGameGeekGameToMechanic", name: "IX_BoardGameGeekGameDefinition_Id", newName: "IX_Id");
            //RenameIndex(table: "dbo.BoardGameGeekGameToMechanic", name: "IX_BoardGameGeekGameMechanic_Id", newName: "IX_BoardGameGeekGameMechanicId");
            //DropPrimaryKey("dbo.BoardGameGeekGameToMechanic");
            //AddPrimaryKey("dbo.BoardGameGeekGameToMechanic", new[] { "Id", "BoardGameGeekGameMechanicId" });
        }
        
        public override void Down()
        {
            //DropPrimaryKey("dbo.BoardGameGeekGameToMechanic");
            //AddPrimaryKey("dbo.BoardGameGeekGameToMechanic", new[] { "BoardGameGeekGameMechanic_Id", "BoardGameGeekGameDefinition_Id" });
            //RenameIndex(table: "dbo.BoardGameGeekGameToMechanic", name: "IX_BoardGameGeekGameMechanicId", newName: "IX_BoardGameGeekGameMechanic_Id");
            //RenameIndex(table: "dbo.BoardGameGeekGameToMechanic", name: "IX_Id", newName: "IX_BoardGameGeekGameDefinition_Id");
            //RenameColumn(table: "dbo.BoardGameGeekGameToMechanic", name: "Id", newName: "BoardGameGeekGameDefinition_Id");
            //RenameColumn(table: "dbo.BoardGameGeekGameToMechanic", name: "BoardGameGeekGameMechanicId", newName: "BoardGameGeekGameMechanic_Id");
            //RenameTable(name: "dbo.BoardGameGeekGameToMechanic", newName: "BoardGameGeekGameMechanicBoardGameGeekGameDefinition");
        }
    }
}
