using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class CollaboratorAchievement : BaseAchievement
    {

        private readonly List<string> _nemestatsCollaboratorsUserIdsList = new List<string>()
        {
            "4817f408-435a-41cc-8592-1a46695ab4f7", //@dsilva609
            "57daa4a5-1506-4155-ad30-bf1e1daf97d2", //@cracker4o
            "80629c07-b8df-4deb-a9e3-5b503ce7d7df", //@jakejgordon
            "c51a515d-14e9-4be9-b877-2445de8e26d9", //@vfportero
            "eaef04f3-ce87-4366-be8c-fcb4bd8339f3", //@mkoch227
            "91534c26-d2e7-40b7-8101-dbd9782fc77e", //@mgerdes11
            "f773124f-541f-4cba-8a60-91dc7bbd8e40" //@roricabanes
        };
        public CollaboratorAchievement(IDataContext dataContext) : base(dataContext)
        {

        }

        public override AchievementId Id => AchievementId.Collaborator;

        public override AchievementGroup Group => AchievementGroup.NotApply;

        public override string Name => "NemeStats Collaborator";

        public override string DescriptionFormat => "This Achievement is earned for all the GitHub NemeStats Contributors... without them this website can not exist!!.";

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

            var player = DataContext.FindById<Player>(playerId);

            if (_nemestatsCollaboratorsUserIdsList.Contains(player.ApplicationUserId))
            {
                result.PlayerProgress = 1;
                result.LevelAwarded = AchievementLevel.Gold;
            }
            return result;
        }
    }
}