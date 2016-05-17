using System;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Models;
using UI.Models.Achievements;

namespace UI.Transformations
{
    public class PlayerAchievementViewModelBuilder : IPlayerAchievementViewModelBuilder
    {
        private readonly IAchievementViewModelBuilder _achievementViewModelBuilder;

        public PlayerAchievementViewModelBuilder(IAchievementViewModelBuilder achievementViewModelBuilder)
        {
            _achievementViewModelBuilder = achievementViewModelBuilder;
        }

        public PlayerAchievementViewModel Build(PlayerAchievement playerAchievement)
        {
            Validate(playerAchievement);

            var achievement = AchievementFactory.GetAchievementById(playerAchievement.AchievementId);
            return new PlayerAchievementViewModel
            {
                AchievementLevel = playerAchievement.AchievementLevel,
                Achievement = _achievementViewModelBuilder.Build(achievement),
                RelatedEntities = playerAchievement.RelatedEntities,
                PlayerId = playerAchievement.PlayerId,
                PlayerName = playerAchievement.Player.Name,
                DateCreated = playerAchievement.DateCreated,
                LastUpdatedDate = playerAchievement.LastUpdatedDate,
                PlayerProgress = achievement.IsAwardedForThisPlayer(playerAchievement.PlayerId).PlayerProgress
            };
        }

        private void Validate(PlayerAchievement playerAchievement)
        {
            if (playerAchievement == null)
            {
                throw new ArgumentNullException(nameof(playerAchievement));
            }
        }
    }
}