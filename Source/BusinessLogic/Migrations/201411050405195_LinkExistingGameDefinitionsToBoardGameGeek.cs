using BusinessLogic.Logic.GameDefinitions;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkExistingGameDefinitionsToBoardGameGeek : DbMigration
    {
        public override void Up()
        {
            //since the model has changed, since this migration was originally written, this is no longer valid.
            //BoardGameGeekDataLinker dataLinker = new BoardGameGeekDataLinker();
            //dataLinker.CleanUpExistingRecords();
        }
        
        public override void Down()
        {
        }
    }
}
