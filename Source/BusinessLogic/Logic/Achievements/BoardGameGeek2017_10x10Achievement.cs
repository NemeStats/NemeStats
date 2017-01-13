using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class BoardGameGeek2017_10x10Achievement : BoardGameGeek_10x10Achievement
    {
        public BoardGameGeek2017_10x10Achievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.BoardGameGeek2017_10x10;
        public override string DescriptionFormat => "Completed the BoardGameGeek 2017 10x10 challenge by playing {0} games 10 times each in 2017: https://boardgamegeek.com/geeklist/218459/2017-challenge-play-10-games-10-times-each";
        public override int Year { get; } = 2016;
    }
}