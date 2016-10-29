namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserDeviceAuthToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserDeviceAuthToken",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthenticationToken = c.String(nullable: false, maxLength: 128),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        DeviceId = c.String(maxLength: 128),
                        AuthenticationTokenExpirationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.AuthenticationToken, unique: true, name: "IX_Authentication_Token")
                .Index(t => new { t.ApplicationUserId, t.DeviceId }, unique: true);

            //--copy over all existing auth tokens to the new table
            Sql(@"INSERT INTO UserDeviceAuthToken (AuthenticationToken, ApplicationUserId, DeviceId, AuthenticationTokenExpirationDate)
                SELECT AuthenticationToken, Id, NULL AS DeviceId, AuthenticationTokenExpirationDate FROM AspNetUsers WHERE AuthenticationToken IS NOT NULL");

            DropColumn("dbo.AspNetUsers", "AuthenticationToken");
            DropColumn("dbo.AspNetUsers", "AuthenticationTokenExpirationDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AuthenticationTokenExpirationDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "AuthenticationToken", c => c.String());
            DropForeignKey("dbo.UserDeviceAuthToken", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserDeviceAuthToken", new[] { "ApplicationUserId", "DeviceId" });
            DropIndex("dbo.UserDeviceAuthToken", "IX_Authentication_Token");
            DropTable("dbo.UserDeviceAuthToken");
        }
    }
}
