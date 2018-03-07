namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTpoGamesEverVotableFeature : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[VotableFeature] ([Id],[NumberOfUpvotes],[NumberOfDownVotes],[DateCreated],[DateModified]) VALUES('TopGamesEver', 0, 0, GETDATE(), GETDATE())");
        }

        public override void Down()
        {
            Sql("DELETE FROM [dbo].[VotableFeature] WHERE Id = 'TopGamesEver'");
        }
    }
}
