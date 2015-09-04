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
    
    public partial class UniqueConstraintOnUserGamingGroup : DbMigration
    {
        public override void Up()
        {
            this.Sql("DELETE FROM dbo.UserGamingGroup;");

            DropIndex("dbo.UserGamingGroup", new[] { "ApplicationUserId" });
            DropIndex("dbo.UserGamingGroup", new[] { "GamingGroupId" });
            CreateIndex("dbo.UserGamingGroup", new[] { "ApplicationUserId", "GamingGroupId" }, unique: true, name: "IX_USERID_AND_GAMING_GROUPID");
            
            this.Sql(@"INSERT INTO UserGamingGroup (ApplicationUserId, GamingGroupId) 
              (SELECT Id, CurrentGamingGroupid FROM AspNetUsers 
              WHERE NOT EXISTS (SELECT 1 FROM UserGamingGroup UGG2 WHERE UGG2.ApplicationUserId = AspNetUsers.Id 
              AND UGG2.GamingGroupId = AspNetUsers.CurrentGamingGroupId))");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserGamingGroup", "IX_USERID_AND_GAMING_GROUPID");
            CreateIndex("dbo.UserGamingGroup", "GamingGroupId");
            CreateIndex("dbo.UserGamingGroup", "ApplicationUserId");
        }
    }
}
