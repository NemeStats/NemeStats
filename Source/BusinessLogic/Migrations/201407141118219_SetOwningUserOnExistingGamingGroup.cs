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
    
    public partial class SetOwningUserOnExistingGamingGroup : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE GamingGroup SET OwningUserId = (SELECT TOP 1 Id FROM AspNetUsers)");
            DropForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.AspNetUsers");
            DropIndex("dbo.GamingGroup", new[] { "OwningUserId" });
            AlterColumn("dbo.GamingGroup", "OwningUserId", c => c.String(maxLength: 128, nullable: false));
            CreateIndex("dbo.GamingGroup", "OwningUserId");
            AddForeignKey("dbo.GamingGroup", "OwningUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            Sql("UPDATE GamingGroup SET OwningUserId = NULL");
            AlterColumn("dbo.GamingGroup", "OwningUserId", c => c.String(maxLength: 128, nullable: true));
        }
    }
}
