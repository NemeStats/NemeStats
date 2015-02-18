namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVotableFeature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VotableFeature",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 255),
                        FeatureDescription = c.String(),
                        NumberOfUpvotes = c.Int(nullable: false),
                        NumberOfDownvotes = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            this.Sql("INSERT INTO [dbo].[VotableFeature] ([Id], [FeatureDescription] ,[NumberOfUpvotes] ,[NumberOfDownvotes] ,[DateCreated] ,[DateModified]) VALUES ('TopGlobalPlayers', 'Widget for showing top Players based on number of games played', 0, 0, GETUTCDATE(), GETUTCDATE())");
            this.Sql("INSERT INTO [dbo].[VotableFeature] ([Id], [FeatureDescription] ,[NumberOfUpvotes] ,[NumberOfDownvotes] ,[DateCreated] ,[DateModified]) VALUES ('TopGlobalGamingGroups', 'Widget for showing top Gaming Groups based on number of games played', 0, 0, GETUTCDATE(), GETUTCDATE())");
            this.Sql("INSERT INTO [dbo].[VotableFeature] ([Id], [FeatureDescription] ,[NumberOfUpvotes] ,[NumberOfDownvotes] ,[DateCreated] ,[DateModified]) VALUES ('RecentGlobalGames', 'Widget for showing the games that were most recently played based on the date played (not the date recorded)', 0, 0, GETUTCDATE(), GETUTCDATE())");
            this.Sql("INSERT INTO [dbo].[VotableFeature] ([Id], [FeatureDescription] ,[NumberOfUpvotes] ,[NumberOfDownvotes] ,[DateCreated] ,[DateModified]) VALUES ('RecentGlobalNemesisChanges', 'Widget for showing recent Nemesis changes', 0, 0, GETUTCDATE(), GETUTCDATE())");
        }
        
        public override void Down()
        {
            DropTable("dbo.VotableFeature");
        }
    }
}
