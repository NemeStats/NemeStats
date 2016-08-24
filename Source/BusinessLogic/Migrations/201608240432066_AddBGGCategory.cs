namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBGGCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BGGGameToCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardGameGeekGameDefinitionId = c.Int(nullable: false),
                        BoardGameGeekGameCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BoardGameGeekGameDefinition", "IsExpansion", c => c.Boolean(nullable: false));
            AddColumn("dbo.BoardGameGeekGameDefinition", "Rank", c => c.Int());

            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1009, 'Abstract Strategy')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1032, 'Action / Dexterity')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1022, 'Adventure')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2726, 'Age of Reason')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1048, 'American Civil War')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1108, 'American Indian Wars')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1075, 'American Revolutionary War')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1055, 'American West')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1050, 'Ancient')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1089, 'Animals')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1052, 'Arabian')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2650, 'Aviation / Flight')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1023, 'Bluffing')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1117, 'Book')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1002, 'Card Game')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1041, 'Children''s Game')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1029, 'City Building')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1102, 'Civil War')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1015, 'Civilization')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1044, 'Collectible Components')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1116, 'Comic Book / Strip')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1039, 'Deduction')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1017, 'Dice')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1021, 'Economic')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1094, 'Educational')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1072, 'Electronic')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1084, 'Environmental')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1042, 'Expansion for Base-game')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1020, 'Exploration')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2687, 'Fan Expansion')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1010, 'Fantasy')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1013, 'Farming')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1046, 'Fighting')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1119, 'Game System')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1024, 'Horror')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1079, 'Humor')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1088, 'Industry / Manufacturing')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1091, 'Korean War')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1033, 'Mafia')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1104, 'Math')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1118, 'Mature / Adult')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1059, 'Maze')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2145, 'Medical')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1035, 'Medieval')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1045, 'Memory')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1047, 'Miniatures')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1069, 'Modern Warfare')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1064, 'Movies / TV / Radio theme')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1040, 'Murder/Mystery')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1054, 'Music')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1082, 'Mythology')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1051, 'Napoleonic')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1008, 'Nautical')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1026, 'Negotiation')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1093, 'Novel-based')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1098, 'Number')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1030, 'Party Game')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2725, 'Pike and Shot')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1090, 'Pirates')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1001, 'Political')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2710, 'Post-Napoleonic')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1036, 'Prehistoric')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1120, 'Print & Play')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1028, 'Puzzle')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1031, 'Racing')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1037, 'Real-time')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1115, 'Religious')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1070, 'Renaissance')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1016, 'Science Fiction')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1113, 'Space Exploration')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1081, 'Spies/Secret Agents')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1038, 'Sports')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1086, 'Territory Building')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1034, 'Trains')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1011, 'Transportation')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1097, 'Travel')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1027, 'Trivia')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1101, 'Video Game Theme')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1109, 'Vietnam War')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1019, 'Wargame')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1025, 'Word Game')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1065, 'World War I')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(1049, 'World War II')");
            this.Sql("insert into [dbo].[BoardGameGeekGameCategory] ([Id], [CategoryName]) values(2481, 'Zombies')");
        }

        public override void Down()
        {
            DropColumn("dbo.BoardGameGeekGameDefinition", "Rank");
            DropColumn("dbo.BoardGameGeekGameDefinition", "IsExpansion");
            DropTable("dbo.BGGGameToCategory");
        }
    }
}
