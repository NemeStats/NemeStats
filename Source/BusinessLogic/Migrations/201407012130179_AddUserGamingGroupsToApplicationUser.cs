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
    
    public partial class AddUserGamingGroupsToApplicationUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserGamingGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        GamingGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GamingGroup", t => t.GamingGroupId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.GamingGroupId);

            Sql("INSERT INTO [dbo].[UserGamingGroup] (ApplicationUserId, GamingGroupId)"
                + "SELECT Id, (SELECT TOP 1 Id FROM GamingGroup) FROM AspNetUsers");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGamingGroup", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserGamingGroup", "GamingGroupId", "dbo.GamingGroup");
            DropIndex("dbo.UserGamingGroup", new[] { "GamingGroupId" });
            DropIndex("dbo.UserGamingGroup", new[] { "ApplicationUserId" });
            DropTable("dbo.UserGamingGroup");
        }
    }
}
