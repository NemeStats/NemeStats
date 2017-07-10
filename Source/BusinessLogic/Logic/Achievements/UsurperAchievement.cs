using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;
using BusinessLogic.DataAccess.Repositories;

namespace BusinessLogic.Logic.Achievements
{
    public class UsurperAchievement : BaseAchievement
    {
        private readonly IChampionRepository _championRepository;

        public UsurperAchievement(IDataContext dataContext, IChampionRepository championRepository) : base(dataContext)
        {
            _championRepository = championRepository;
        }

        public override AchievementId Id => AchievementId.Usurper;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Usurper";

        public override string DescriptionFormat => "This Achievement is earned by becoming the Champion of {0} different games where someone else was the Champion first.";

        public override string IconClass => "fa fa-hand-o-down";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var gamesForUsurperAchievement = _championRepository.GetUsurperAchievementData(playerId);

            result.RelatedEntities = gamesForUsurperAchievement;
            result.PlayerProgress = gamesForUsurperAchievement.Count;

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
