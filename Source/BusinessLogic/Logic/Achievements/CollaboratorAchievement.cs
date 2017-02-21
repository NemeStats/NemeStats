using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class CollaboratorAchievement : BaseAchievement
    {

        public CollaboratorAchievement(IDataContext dataContext) : base(dataContext)
        {

        }

        public override AchievementId Id => AchievementId.Collaborator;

        public override AchievementGroup Group => AchievementGroup.NotApplicable;

        public override string Name => "NemeStats Collaborator";

        public override string DescriptionFormat => "This Achievement is earned for all the GitHub NemeStats Contributors... without them this website can not exist!!";

        public override string IconClass => "fa fa-github";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Gold, 1}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {

            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            //TODO this will go away. Need to refactor
            if (this.Winners.Value.Any(w => w.PlayerId == playerId))
            {
                result.PlayerProgress = 1;
                result.LevelAwarded = AchievementLevel.Gold;
            }
            return result;
        }
    }
}