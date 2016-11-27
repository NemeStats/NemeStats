using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class HolidayGamerAchievement : BaseAchievement
    {
        public HolidayGamerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.HolidayGamer;

        public override AchievementGroup Group => AchievementGroup.PlayedGame;

        public override string Name => "Holiday Gamer";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} Games anytime on or between December 24th and January 1st";

        public override string IconClass => "fa fa-gift";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 10},
            {AchievementLevel.Gold, 30}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var gamesPlayed =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(pgr => pgr.PlayerId == playerId
                                  && ((pgr.PlayedGame.DatePlayed.Month == 12 && pgr.PlayedGame.DatePlayed.Day >= 24)
                                      || (pgr.PlayedGame.DatePlayed.Month == 1 && pgr.PlayedGame.DatePlayed.Day == 1)))
                    .Select(pg => pg.PlayedGameId).ToList();

            result.RelatedEntities = gamesPlayed;
            result.PlayerProgress = gamesPlayed.Count;

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
