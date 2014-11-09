namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateCreated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GameDefinition", "DateCreated", c => c.DateTime(nullable: true, defaultValue: DateTime.UtcNow));
            Sql("UPDATE GameDefinition SET DateCreated = (SELECT GamingGroup.DateCreated FROM GamingGroup WHERE GamingGroup.Id = GameDefinition.GamingGroupId)");

            AddColumn("dbo.PlayedGame", "DateCreated", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
            this.Sql("UPDATE PlayedGame SET DateCreated = "
                + "(SELECT CASE WHEN PG2.DatePlayed > GamingGroup.DateCreated THEN PlayedGame.DatePlayed ELSE GamingGroup.DateCreated END"
                + " FROM PlayedGame AS PG2 INNER JOIN GamingGroup ON PG2.GamingGroupId = GamingGroup.Id AND PlayedGame.Id = PG2.Id)");

            AddColumn("dbo.Player", "DateCreated", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
            Sql("UPDATE Player SET DateCreated = (SELECT GamingGroup.DateCreated FROM GamingGroup WHERE GamingGroup.Id = Player.GamingGroupId)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "DateCreated");
            DropColumn("dbo.PlayedGame", "DateCreated");
            DropColumn("dbo.GameDefinition", "DateCreated");
        }
    }
}
