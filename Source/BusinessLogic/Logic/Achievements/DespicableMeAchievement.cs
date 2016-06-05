using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DespicableMeAchievement : BaseAchievement
    {
        public DespicableMeAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.DespicableMe;
        public override AchievementGroup Group => AchievementGroup.Player;
        public override string Name => "Despicable Me";
        public override string DescriptionFormat => "Be the Nemesis of {0} players on your Gaming Group";
        public override string IconClass => "ns-icon-eye-minion";
        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 3},
            {AchievementLevel.Silver, 5},
            {AchievementLevel.Gold, 10}
        };
        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var minions = DataContext.GetQueryable<Nemesis>()
                .Include(n => n.MinionPlayer)
                .Where(n => n.NemesisPlayerId == playerId && n.MinionPlayer.NemesisId == playerId)
                .Select(n => n.MinionPlayerId).
                ToList();



            result.PlayerProgress = minions.Count;
            result.RelatedEntities = minions;


            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);

            return result;
        }
    }
}