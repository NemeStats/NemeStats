namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeGamingGroupInvitationGuidIdentity : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.GamingGroupInvitation");
            AlterColumn("dbo.GamingGroupInvitation", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newid()"));
            AddPrimaryKey("dbo.GamingGroupInvitation", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.GamingGroupInvitation");
            AlterColumn("dbo.GamingGroupInvitation", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.GamingGroupInvitation", "Id");
        }
    }
}
