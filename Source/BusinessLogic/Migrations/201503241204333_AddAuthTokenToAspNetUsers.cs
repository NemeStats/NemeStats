namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthTokenToAspNetUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AuthenticationToken", c => c.String());
            AddColumn("dbo.AspNetUsers", "AuthenticationTokenExpirationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AuthenticationTokenExpirationDate");
            DropColumn("dbo.AspNetUsers", "AuthenticationToken");
        }
    }
}
