namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewBggFieldsToSync : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekGameDefinition", "MaxPlayers", c => c.Int());
            AddColumn("dbo.BoardGameGeekGameDefinition", "MinPlayers", c => c.Int());
            AddColumn("dbo.BoardGameGeekGameDefinition", "PlayingTime", c => c.Int());
            AddColumn("dbo.BoardGameGeekGameDefinition", "AverageWeight", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "AverageWeight");
            DropColumn("dbo.BoardGameGeekGameDefinition", "PlayingTime");
            DropColumn("dbo.BoardGameGeekGameDefinition", "MinPlayers");
            DropColumn("dbo.BoardGameGeekGameDefinition", "MaxPlayers");
        }
    }
}
