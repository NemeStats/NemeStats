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
    
    public partial class AddedRegistrationInfoToGamingGroupInvitation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GamingGroupInvitation", "RegisteredUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.GamingGroupInvitation", "DateRegistered", c => c.DateTime());
            CreateIndex("dbo.GamingGroupInvitation", "RegisteredUserId");
            AddForeignKey("dbo.GamingGroupInvitation", "RegisteredUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GamingGroupInvitation", "RegisteredUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GamingGroupInvitation", new[] { "RegisteredUserId" });
            DropColumn("dbo.GamingGroupInvitation", "DateRegistered");
            DropColumn("dbo.GamingGroupInvitation", "RegisteredUserId");
        }
    }
}
