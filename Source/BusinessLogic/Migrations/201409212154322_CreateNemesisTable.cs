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
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateNemesisTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nemesis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MinionPlayerId = c.Int(nullable: false),
                        NemesisPlayerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false, defaultValue: DateTime.UtcNow),
                        NumberOfGamesLost = c.Int(nullable: false),
                        LossPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Player", t => t.MinionPlayerId, cascadeDelete: false)
                .ForeignKey("dbo.Player", t => t.NemesisPlayerId, cascadeDelete: false)
                .Index(t => t.MinionPlayerId)
                .Index(t => t.NemesisPlayerId);
            
            AddColumn("dbo.Player", "NemesisId", c => c.Int());
            CreateIndex("dbo.Player", "NemesisId");
            AddForeignKey("dbo.Player", "NemesisId", "dbo.Nemesis", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Player", "NemesisId", "dbo.Nemesis");
            DropForeignKey("dbo.Nemesis", "NemesisPlayerId", "dbo.Player");
            DropForeignKey("dbo.Nemesis", "MinionPlayerId", "dbo.Player");
            DropIndex("dbo.Nemesis", new[] { "NemesisPlayerId" });
            DropIndex("dbo.Nemesis", new[] { "MinionPlayerId" });
            DropIndex("dbo.Player", new[] { "NemesisId" });
            DropColumn("dbo.Player", "NemesisId");
            DropTable("dbo.Nemesis");
        }
    }
}
