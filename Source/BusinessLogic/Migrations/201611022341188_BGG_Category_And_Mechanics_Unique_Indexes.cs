namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BGG_Category_And_Mechanics_Unique_Indexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.BoardGameGeekGameCategory", "BoardGameGeekGameCategoryId", unique: true, name: "IX_BOARDGAMEGEEKCATEGORYID");
            CreateIndex("dbo.BoardGameGeekGameMechanic", "BoardGameGeekGameMechanicId", unique: true, name: "IX_BOARDGAMEGEEKMECHANICID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.BoardGameGeekGameMechanic", "IX_BOARDGAMEGEEKCATEGORYID");
            DropIndex("dbo.BoardGameGeekGameCategory", "IX_BOARDGAMEGEEKMECHANICID");
        }
    }
}
