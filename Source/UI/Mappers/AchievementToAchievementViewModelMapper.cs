using System.Linq;
using AutoMapper;
using BusinessLogic.Logic.Achievements;
using UI.Models.Achievements;

namespace UI.Mappers
{
    public class AchievementToAchievementViewModelMapper : BaseMapperService<IAchievement, AchievementViewModel>
    {
        
        private readonly PlayerAchievementToPlayerAchievementWinnerViewModelMapper _achievementToPlayerAchievementWinnerViewModelMapper;

        public AchievementToAchievementViewModelMapper(PlayerAchievementToPlayerAchievementWinnerViewModelMapper achievementToPlayerAchievementWinnerViewModelMapper)
        {
            
            _achievementToPlayerAchievementWinnerViewModelMapper = achievementToPlayerAchievementWinnerViewModelMapper;
        }

        static AchievementToAchievementViewModelMapper()
        {
            Mapper.CreateMap<IAchievement, AchievementViewModel>()
                .ForMember(m => m.Winners, o => o.Ignore());
        }

        public override AchievementViewModel Map(IAchievement source)
        {
            var model = base.Map(source);

            model.Winners = source.Winners.Value.Select(w=> _achievementToPlayerAchievementWinnerViewModelMapper.Map(w)).ToList();

            return model;
        }
    }
}