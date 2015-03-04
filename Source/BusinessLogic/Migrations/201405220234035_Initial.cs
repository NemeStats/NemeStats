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
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameDefinition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayedGame",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameDefinitionId = c.Int(nullable: false),
                        NumberOfPlayers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameDefinition", t => t.GameDefinitionId, cascadeDelete: true)
                .Index(t => t.GameDefinitionId);
            
            CreateTable(
                "dbo.PlayerGameResult",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayedGameId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        GameRank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlayedGame", t => t.PlayedGameId, cascadeDelete: true)
                .ForeignKey("dbo.Player", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.PlayedGameId)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.Player",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerGameResult", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.PlayerGameResult", "PlayedGameId", "dbo.PlayedGame");
            DropForeignKey("dbo.PlayedGame", "GameDefinitionId", "dbo.GameDefinition");
            DropIndex("dbo.PlayerGameResult", new[] { "PlayerId" });
            DropIndex("dbo.PlayerGameResult", new[] { "PlayedGameId" });
            DropIndex("dbo.PlayedGame", new[] { "GameDefinitionId" });
            DropTable("dbo.Player");
            DropTable("dbo.PlayerGameResult");
            DropTable("dbo.PlayedGame");
            DropTable("dbo.GameDefinition");
        }
    }
}
