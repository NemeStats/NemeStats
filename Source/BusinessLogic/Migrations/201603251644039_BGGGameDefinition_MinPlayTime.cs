namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BGGGameDefinition_MinPlayTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardGameGeekGameDefinition", "MinPlayTime", c => c.Int());
            RenameColumn("dbo.BoardGameGeekGameDefinition", "PlayingTime", "MaxPlayTime");
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "MinPlayTime");
            RenameColumn("dbo.BoardGameGeekGameDefinition", "MaxPlayTime", "PlayingTime");
        }
    }
}
