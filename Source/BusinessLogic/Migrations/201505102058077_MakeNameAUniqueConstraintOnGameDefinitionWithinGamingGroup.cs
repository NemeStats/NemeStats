namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakeNameAUniqueConstraintOnGameDefinitionWithinGamingGroup : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.GameDefinition", new[] { "GamingGroupId" });

            this.Sql(@";WITH DuplicateGameDefinitionCTE (Id, Name, GamingGroupID) AS
                    (
	                        SELECT MAX(Id) AS Id, Name, GamingGroupid
	                        FROM GameDefinition
	                        GROUP BY Name, GamingGroupId
	                        HAVING COUNT(*) > 1
                    ) 
                    UPDATE GameDefinition Set GameDefinition.Name = (DuplicateGameDefinitionCTE.Name + ' (Duplicate)')
                    FROM GameDefinition INNER JOIN DuplicateGameDefinitionCTE ON GameDefinition.Id = DuplicateGameDefinitionCTE.Id;

                    ;WITH DuplicateGameDefinitionCTE (Id, Name, GamingGroupID) AS
                    (
	                        SELECT MAX(Id) AS Id, Name, GamingGroupid
	                        FROM GameDefinition
	                        GROUP BY Name, GamingGroupId
	                        HAVING COUNT(*) > 1
                    ) 
                    UPDATE GameDefinition Set GameDefinition.Name = (DuplicateGameDefinitionCTE.Name + ' (Duplicate 2)')
                    FROM GameDefinition INNER JOIN DuplicateGameDefinitionCTE ON GameDefinition.Id = DuplicateGameDefinitionCTE.Id;

                    ;WITH DuplicateGameDefinitionCTE (Id, Name, GamingGroupID) AS
                    (
	                        SELECT MAX(Id) AS Id, Name, GamingGroupid
	                        FROM GameDefinition
	                        GROUP BY Name, GamingGroupId
	                        HAVING COUNT(*) > 1
                    ) 
                    UPDATE GameDefinition Set GameDefinition.Name = (DuplicateGameDefinitionCTE.Name + ' (Duplicate 3)')
                    FROM GameDefinition INNER JOIN DuplicateGameDefinitionCTE ON GameDefinition.Id = DuplicateGameDefinitionCTE.Id;");
            
            AlterColumn("dbo.GameDefinition", "Name", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.GameDefinition", new[] { "GamingGroupId", "Name" }, unique: true, name: "IX_ID_AND_NAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.GameDefinition", "IX_ID_AND_NAME");
            AlterColumn("dbo.GameDefinition", "Name", c => c.String(nullable: false));
            CreateIndex("dbo.GameDefinition", "GamingGroupId");
        }
    }
}
