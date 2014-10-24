namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfirmAllEmails : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.AspNetUsers SET EmailConfirmed = 1 WHERE DateCreated < '2014-10-10';");
        }
        
        public override void Down()
        {
        }
    }
}
