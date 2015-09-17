#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
namespace BusinessLogic.Migrations
{
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
