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
    
    public partial class CreateChampionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Champion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GameDefinitionId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        WinPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameDefinition", t => t.GameDefinitionId, cascadeDelete: false)
                .ForeignKey("dbo.Player", t => t.PlayerId, cascadeDelete: false)
                .Index(t => t.GameDefinitionId)
                .Index(t => t.PlayerId);
            
            AddColumn("dbo.GameDefinition", "ChampionId", c => c.Int());
            AddColumn("dbo.GameDefinition", "PreviousChampionId", c => c.Int());
            CreateIndex("dbo.GameDefinition", "ChampionId");
            CreateIndex("dbo.GameDefinition", "PreviousChampionId");
            AddForeignKey("dbo.GameDefinition", "ChampionId", "dbo.Champion", "Id");
            AddForeignKey("dbo.GameDefinition", "PreviousChampionId", "dbo.Champion", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Champion", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.Champion", "GameDefinitionId", "dbo.GameDefinition");
            DropForeignKey("dbo.GameDefinition", "PreviousChampionId", "dbo.Champion");
            DropForeignKey("dbo.GameDefinition", "ChampionId", "dbo.Champion");
            DropIndex("dbo.GameDefinition", new[] { "PreviousChampionId" });
            DropIndex("dbo.GameDefinition", new[] { "ChampionId" });
            DropIndex("dbo.Champion", new[] { "PlayerId" });
            DropIndex("dbo.Champion", new[] { "GameDefinitionId" });
            DropColumn("dbo.GameDefinition", "PreviousChampionId");
            DropColumn("dbo.GameDefinition", "ChampionId");
            DropTable("dbo.Champion");
        }
    }
}
