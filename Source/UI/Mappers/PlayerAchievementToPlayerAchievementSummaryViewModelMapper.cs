using AutoMapper;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Models;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class PlayerAchievementToPlayerAchievementSummaryViewModelMapper : BaseMapperService<PlayerAchievement, PlayerAchievementSummaryViewModel>
    {
        private readonly AchievementToAchievementViewModelMapper _achievementToAchievementViewModelMapper;

        public PlayerAchievementToPlayerAchievementSummaryViewModelMapper(AchievementToAchievementViewModelMapper achievementToAchievementViewModelMapper)
        {
            _achievementToAchievementViewModelMapper = achievementToAchievementViewModelMapper;
        }

        static PlayerAchievementToPlayerAchievementSummaryViewModelMapper()
        {
            Mapper.CreateMap<PlayerAchievement, PlayerAchievementSummaryViewModel>()
                .ForMember(m => m.Achievement, o => o.Ignore());
        }

        public override PlayerAchievementSummaryViewModel Map(PlayerAchievement source)
        {
            var result = base.Map(source);

            var achievement = AchievementFactory.GetAchievementById(source.AchievementId);
            result.Achievement = _achievementToAchievementViewModelMapper.Map(achievement);
            return result;
        }
    }
}