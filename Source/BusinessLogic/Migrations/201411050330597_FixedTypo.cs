using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedTypo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameDefinition", "BoardGameGeekObjectId", c => c.Int());
            DropColumn("dbo.GameDefinition", "BoardGameGeekObjecdtId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameDefinition", "BoardGameGeekObjecdtId", c => c.Int());
            DropColumn("dbo.GameDefinition", "BoardGameGeekObjectId");
        }
    }
}
