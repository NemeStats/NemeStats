using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class BoardGameGeek2016_10x10Achievement : BoardGameGeek_10x10Achievement
    {
        public BoardGameGeek2016_10x10Achievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.BoardGameGeek2016_10x10;
        public override string DescriptionFormat => "Completed the BoardGameGeek 2016 10x10 challenge by playing {0} games 10 times each in 2016: http://boardgamegeek.com/geeklist/201403/2016-challenge-play-10-games-10-times-each";
        public override int Year { get; } = 2016;
    }
}
