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
    
    public partial class PlayerNameIsNoLongerNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Player", "IX_ID_AND_NAME");
            AlterColumn("dbo.Player", "Name", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.Player", new[] { "GamingGroupId", "Name" }, unique: true, name: "IX_ID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Player", "IX_ID_AND_NAME");
            AlterColumn("dbo.Player", "Name", c => c.String(maxLength: 255));
            CreateIndex("dbo.Player", new[] { "GamingGroupId", "Name" }, unique: true, name: "IX_ID_AND_NAME");
        }
    }
}
