using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class PunisherAchievement : BaseAchievement
    {
        public PunisherAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Punisher;

        public override AchievementGroup Group => AchievementGroup.Player;

        public override string Name => "Punisher";

        public override string DescriptionFormat => "This Achievement is earned when a Player becomes the Nemesis of a Player "
            + "who was previously their Nemesis. Sweet, sweet revenge! "
            + " Must overtake {0} different Players.";

        public override string IconClass => "ns-icon-punisher";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 4},
            {AchievementLevel.Gold, 10}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var revengedPlayerIds =
                DataContext
                    .GetQueryable<Nemesis>()
                    .Where(nem => nem.NemesisPlayerId == playerId
                                  && nem.NemesisPlayer.CurrentAndPriorNemeses.Any(
                                      min => min.NemesisPlayerId == nem.MinionPlayerId
                                             && min.DateCreated < nem.DateCreated))
                    .Select(nem => nem.MinionPlayerId)
                    .Distinct()
                    .ToList();

            result.RelatedEntities = revengedPlayerIds;
            result.PlayerProgress = revengedPlayerIds.Count;

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
