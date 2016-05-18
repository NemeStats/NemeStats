using AutoMapper;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Models;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class PlayerAchievementToPlayerAchievementViewModelMapper: BaseMapperService<PlayerAchievement, PlayerAchievementViewModel>
    {
        private readonly AchievementToAchievementViewModelMapper _achievementToAchievementViewModelMapper;

        public PlayerAchievementToPlayerAchievementViewModelMapper(AchievementToAchievementViewModelMapper achievementToAchievementViewModelMapper)
        {
            _achievementToAchievementViewModelMapper = achievementToAchievementViewModelMapper;
        }

        static PlayerAchievementToPlayerAchievementViewModelMapper()
        {
            Mapper.CreateMap<PlayerAchievement, PlayerAchievementViewModel>()
                .ForMember(m => m.Achievement, o => o.Ignore())
                .ForMember(m => m.PlayerProgress, o => o.Ignore());
        }

        public override PlayerAchievementViewModel Map(PlayerAchievement source)
        {
            var result = base.Map(source);

            var achievement = AchievementFactory.GetAchievementById(source.AchievementId);
            result.Achievement = _achievementToAchievementViewModelMapper.Map(achievement);
            result.PlayerProgress = achievement.IsAwardedForThisPlayer(result.PlayerId).PlayerProgress;

            return result;
        }
    }
}