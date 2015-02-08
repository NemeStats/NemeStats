namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVotableFeature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VotableFeature",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FeatureDescription = c.String(),
                        NumberOfUpvotes = c.Int(nullable: false),
                        NumberOfDownvotes = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VotableFeature");
        }
    }
}
